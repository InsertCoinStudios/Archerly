
using System.Diagnostics.CodeAnalysis;
using archerly.core.extensions;
namespace archerly.core.hunts;

public class SessionManager
{
    private readonly Dictionary<string, SessionEntry<Hunt>> _hunts = new();
    private readonly Dictionary<string, SessionEntry<PendingHunt>> _pendingHunts = new();
    public long Count => CalculateCount();
    private long _allSessions;
    private long _softDeletedSessions;
    private readonly Lock _lock = new();
    private readonly Timer? _cleanupTimer;
    private readonly TimeSpan _cleanupInterval;

    private SessionManager(long intervalInMinutes)
    {
        _cleanupInterval = TimeSpan.FromMinutes(intervalInMinutes);
        _cleanupTimer = new Timer(_ => Cleanup(), null, _cleanupInterval, _cleanupInterval);
    }

    public SessionManager()
    {
        _cleanupInterval = TimeSpan.Zero;
        _cleanupTimer = null;
    }

    public static SessionManager WithAutomaticCleanup(long intervalInMinutes)
    {
        return new SessionManager(intervalInMinutes);
    }

    private long CalculateCount()
    {
        var res = _allSessions - _softDeletedSessions;
        if (res >= 0)
        {
            return res;
        }
        else
        {
            // log this
            return 0;
        }
    }

    public void AddHunt(Hunt hunt)
    {
        ArgumentNullException.ThrowIfNull(hunt);

        lock (_lock)
        {
            _hunts[hunt.SessionId] = new SessionEntry<Hunt>(hunt);
            _allSessions++;
        }
    }

    public void AddPendingHunt(PendingHunt pendingHunt)
    {
        ArgumentNullException.ThrowIfNull(pendingHunt);

        lock (_lock)
        {
            _pendingHunts[pendingHunt.SessionId] = new SessionEntry<PendingHunt>(pendingHunt);
            _allSessions++;
        }
    }

    /// <summary>
    /// Marks the active hunt with the given session ID as deleted (soft delete).
    /// </summary>
    /// <param name="sessionId">The unique identifier of the hunt to remove.</param>
    public void RemoveHunt(string sessionId)
    {
        lock (_lock)
        {
            if (_hunts.TryGetValue(sessionId, out var entry))
            {
                entry.SoftDelete();
                _softDeletedSessions++;
            }
        }
    }

    /// <summary>
    /// Marks the pending hunt with the given session ID as deleted (soft delete).
    /// </summary>
    /// <param name="sessionId">The unique identifier of the pending hunt to remove.</param>
    public void RemovePendingHunt(string sessionId)
    {
        lock (_lock)
        {
            if (_pendingHunts.TryGetValue(sessionId, out var entry))
            {
                entry.SoftDelete();
                _softDeletedSessions++;
            }
        }
    }

    /// <summary>
    /// Marks both the active hunt and the pending hunt with the given session ID as deleted (soft delete).
    /// </summary>
    /// <param name="sessionId">The unique identifier of the hunt or pending hunt to remove.</param>
    public void Remove(string sessionId)
    {
        RemoveHunt(sessionId);
        RemovePendingHunt(sessionId);
    }

    public void TransitionFromPending(Hunt hunt)
    {
        RemovePendingHunt(hunt.SessionId);
        AddHunt(hunt);
    }

    /// <summary>
    /// Activates a pending hunt session and transitions it into an active hunt.
    /// </summary>
    /// <param name="sessionId">
    /// The unique identifier of the pending hunt session to activate.
    /// </param>
    /// <exception cref="SessionNotFoundException">
    /// Thrown when no pending hunt exists for the specified <paramref name="sessionId"/>.
    /// </exception>
    /// <exception cref="SessionDeletedException">
    /// Thrown when the pending hunt has been soft-deleted and can no longer be activated.
    /// </exception>
    /// <exception cref="HuntAlreadyActivatedException">
    /// Thrown when the pending hunt has already been activated.
    /// </exception>
    /// <exception cref="ScoringVariantNotSetException">
    /// Thrown when the pending hunt settings do not contain a scoring variant.
    /// </exception>
    /// <exception cref="CourseNotSetException">
    /// Thrown when the pending hunt settings do not contain a selected course.
    /// </exception>
    public void ActivateSession(string sessionId)
    {
        var pending = GetPendingHunt(sessionId);
        pending.Activate();
    }

    /// <summary>
    /// Sets the course for a pending hunt identified by the given session ID.
    /// </summary>
    /// <param name="sessionId">The session identifier of the pending hunt.</param>
    /// <param name="courseId">The unique identifier of the course to assign.</param>
    /// <exception cref="SessionNotFoundException">
    /// Thrown when no pending hunt exists for the specified session ID.
    /// </exception>
    /// <exception cref="SessionDeletedException">
    /// Thrown when the pending hunt has been soft-deleted and can no longer be modified.
    /// </exception>
    public void SetCourse(string sessionId, Guid courseId)
    {
        var pending = GetPendingHunt(sessionId);
        // retrieve course by GUId from db
        // TODO: Replace with call to the repository
        var course = new Course(courseId);
        pending.Settings.SelectedCourse = course;
    }

    /// <summary>
    /// Sets the scoring variant for a pending hunt identified by the given session ID.
    /// </summary>
    /// <param name="sessionId">The session identifier of the pending hunt.</param>
    /// <param name="scoringVariant">
    /// An integer value representing the scoring variant to apply.
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="scoringVariant"/> is not a valid value of <see cref="ShotType"/>.
    /// </exception>
    /// <exception cref="SessionNotFoundException">
    /// Thrown when no pending hunt exists for the specified session ID.
    /// </exception>
    /// <exception cref="SessionDeletedException">
    /// Thrown when the pending hunt has been soft-deleted and can no longer be modified.
    /// </exception>
    public void SetScoringVariant(string sessionId, int scoringVariant)
    {
        ArgumentOutOfRangeException.ThrowIfInvalidEnum<ShotType, int>(scoringVariant, nameof(scoringVariant));
        var pending = GetPendingHunt(sessionId);
        if (!scoringVariant.TryToEnum(out ShotType variant))
        {
            // log this the exception did not get thrown
        }
        pending.Settings.ScoringVariant = variant;
    }

    public void PlayerJoined(string sessionId, Guid playerId)
    {
        var session = GetSession(sessionId);
        PlayerList.HandleGuid action = (id) => { };
        if (session.IsHunt())
        {
            action = session.Hunt.Players.Add;
        }
        if (session.IsPending())
        {
            action = session.Pending.Players.Add;
        }
        action(playerId);
    }

    public void PlayerLeft(string sessionId, Guid playerId)
    {
        var session = GetSession(sessionId);
        PlayerList.HandleGuid action = (id) => { };
        if (session.IsHunt())
        {
            action = session.Hunt.Players.Remove;
        }
        if (session.IsPending())
        {
            action = session.Pending.Players.Remove;
        }
        action(playerId);
    }

    public void RegisterShot(string sessionId, Guid playerId, Guid animalId, long points)
    {
        var hunt = GetHunt(sessionId);
        hunt.Scores.RegisterShot(playerId, animalId, points);
    }


    private SessionReference GetSession(string sessionId)
    {
        lock (_lock)
        {
            if (_hunts.TryGetValue(sessionId, out var huntEntry) && !huntEntry.IsDeleted())
            {
                return SessionReference.FromHunt(huntEntry.Value);
            }

            if (_pendingHunts.TryGetValue(sessionId, out var pendingEntry) && !pendingEntry.IsDeleted())
            {
                return SessionReference.FromPending(pendingEntry.Value);
            }

            return SessionReference.Empty();
        }
    }

    /// <summary>
    /// Retrieves a <see cref="Hunt"/> associated with the specified session ID.
    /// </summary>
    /// <param name="sessionId">The unique identifier of the hunt session.</param>
    /// <returns>
    /// The <see cref="Hunt"/> instance associated with the given session ID.
    /// </returns>
    /// <exception cref="SessionNotFoundException">
    /// Thrown when no hunt exists for the specified <paramref name="sessionId"/>.
    /// </exception>
    /// <exception cref="SessionDeletedException">
    /// Thrown when the hunt exists but has been soft-deleted and is no longer accessible.
    /// </exception>
    public Hunt GetHunt(string sessionId)
    {
        lock (_lock)
        {
            if (!_hunts.TryGetValue(sessionId, out var entry))
            {
                throw new SessionNotFoundException(sessionId);
            }
            if (entry.IsDeleted())
            {
                throw new SessionDeletedException(sessionId);
            }
            else
            {
                return entry.Value;
            }
        }
    }

    /// <summary>
    /// Retrieves a <see cref="PendingHunt"/> associated with the specified session ID.
    /// </summary>
    /// <param name="sessionId">The unique identifier of the pending hunt session.</param>
    /// <returns>
    /// The <see cref="PendingHunt"/> instance associated with the given session ID.
    /// </returns>
    /// <exception cref="SessionNotFoundException">
    /// Thrown when no pending hunt exists for the specified <paramref name="sessionId"/>.
    /// </exception>
    /// <exception cref="SessionDeletedException">
    /// Thrown when the pending hunt exists but has been soft-deleted and is no longer accessible.
    /// </exception>
    public PendingHunt GetPendingHunt(string sessionId)
    {
        lock (_lock)
        {
            if (!_pendingHunts.TryGetValue(sessionId, out var entry))
            {
                throw new SessionNotFoundException(sessionId);
            }
            if (entry.IsDeleted())
            {
                throw new SessionDeletedException(sessionId);
            }
            else
            {
                return entry.Value;
            }
        }
    }

    // Garbage Collector Function
    public void Cleanup()
    {
        lock (_lock)
        {
            // Count how many are being removed from hunts
            int huntsRemoved = _hunts.Count(kvp => kvp.Value.IsDeleted());
            int pendingRemoved = _pendingHunts.Count(kvp => kvp.Value.IsDeleted());

            // Remove them
            _hunts.RemoveAll(kvp => kvp.Value.IsDeleted());
            _pendingHunts.RemoveAll(kvp => kvp.Value.IsDeleted());

            // Update counters
            _allSessions -= huntsRemoved + pendingRemoved;
            _softDeletedSessions -= huntsRemoved + pendingRemoved;
        }
    }

    public void StopCleanUp()
    {
        _cleanupTimer?.Dispose();
    }

    public void Dispose()
    {
        _cleanupTimer?.Dispose();
    }


    internal class SessionEntry<T>
    {
        /// <summary>
        /// Gets the underlying session value if it has not been soft-deleted.
        /// </summary>
        /// <remarks>
        /// When <see cref="IsDeleted"/> is <c>false</c>, this property returns the stored value and is guaranteed to be non-null.
        /// When <see cref="IsDeleted"/> is <c>true</c>, this property returns <c>null</c> to indicate the value is no longer accessible.
        /// </remarks>
        /// <value>
        /// The stored value of type <typeparamref name="T"/> if not deleted; otherwise, <c>null</c>.
        /// </value>
        public T? Value
        {
            get
            {
                if (_isDeleted)
                {
                    return default;
                }
                else
                {
                    return _val;
                }
            }
        }
        private readonly T _val;
        private bool _isDeleted;
        [MemberNotNullWhen(false, nameof(Value))]
        public bool IsDeleted()
        {
            return _isDeleted;
        }

        public SessionEntry(T value)
        {
            _val = value ?? throw new ArgumentNullException(nameof(value));
            _isDeleted = false;
        }

        public void SoftDelete()
        {
            _isDeleted = true;
        }
    }

    internal class SessionReference
    {
        private readonly Hunt? _hunt;
        private readonly PendingHunt? _pending;

        private SessionReference(Hunt? hunt, PendingHunt? pending)
        {
            if (hunt is not null && pending is not null)
            {
                throw new ArgumentException("Cannot create a SessionReference with both a Hunt and a PendingHunt. Only one may be non-null.");
            }
            _hunt = hunt;
            _pending = pending;
        }

        public Hunt? Hunt
        {
            get
            {
                // Only allow access if Pending is null
                if (_pending is not null)
                {
                    return null;
                }
                return _hunt;
            }
        }

        public PendingHunt? Pending
        {
            get
            {
                // Only allow access if Hunt is null
                if (_hunt is not null)
                {
                    return null;
                }
                return _pending;
            }
        }

        /// <summary>
        /// Returns true if this reference contains a Hunt.
        /// </summary>
        [MemberNotNullWhen(true, nameof(Hunt))]
        public bool IsHunt()
        {
            return _hunt is not null;
        }

        /// <summary>
        /// Returns true if this reference contains a PendingHunt.
        /// </summary>
        [MemberNotNullWhen(true, nameof(Pending))]
        public bool IsPending()
        {
            return _pending is not null;
        }

        public Type? GetSessionType()
        {
            if (_hunt is not null)
            {
                return typeof(Hunt);
            }
            if (_pending is not null)
            {
                return typeof(PendingHunt);
            }
            return null;
        }

        internal static SessionReference FromHunt(Hunt hunt)
        {
            return new SessionReference(hunt, null);
        }

        internal static SessionReference FromPending(PendingHunt pending)
        {
            return new SessionReference(null, pending);
        }

        internal static SessionReference Empty()
        {
            return new SessionReference(null, null);
        }
    }

}

public sealed class SessionNotFoundException : Exception
{
    public string SessionId { get; }

    public SessionNotFoundException(string sessionId)
        : base($"Session '{sessionId}' does not exist.")
    {
        SessionId = sessionId;
    }
}

public sealed class SessionDeletedException : Exception
{
    public string SessionId { get; }

    public SessionDeletedException(string sessionId)
        : base($"Session '{sessionId}' has been deleted.")
    {
        SessionId = sessionId;
    }
}