
using System.Diagnostics.CodeAnalysis;
using archerly.core.extensions;
using archerly.metrics;
using Microsoft.AspNetCore.Http;
using Serilog;
namespace archerly.core.hunts;

public class SessionManager : IDisposable
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
            MetricsRegistry.HuntGauge.Inc();
            _allSessions++;
        }
    }

    public void AddPendingHunt(PendingHunt pendingHunt)
    {
        ArgumentNullException.ThrowIfNull(pendingHunt);

        lock (_lock)
        {
            _pendingHunts[pendingHunt.SessionId] = new SessionEntry<PendingHunt>(pendingHunt);
            MetricsRegistry.PendingHuntGauge.Inc();
            _allSessions++;
        }
    }

    /// <summary>
    /// Marks the active hunt with the given session ID as deleted (soft delete).
    /// </summary>
    /// <param name="sessionId">The unique identifier of the hunt to remove.</param>
    public void RemoveHunt(string sessionId)
    {
        //lock (_lock)
        //{
        //    if (_hunts.TryGetValue(sessionId, out var entry))
        //    {
        //        entry.SoftDelete();
        //        MetricsRegistry.HuntGauge.Dec();
        //        MetricsRegistry.SoftDeletedSessionGauge.Inc();
        //        _softDeletedSessions++;
        //    }
        //}
    }

    /// <summary>
    /// Marks the pending hunt with the given session ID as deleted (soft delete).
    /// </summary>
    /// <param name="sessionId">The unique identifier of the pending hunt to remove.</param>
    public void RemovePendingHunt(string sessionId)
    {
        //lock (_lock)
        //{
        //    if (_pendingHunts.TryGetValue(sessionId, out var entry))
        //    {
        //        entry.SoftDelete();
        //        MetricsRegistry.PendingHuntGauge.Dec();
        //        MetricsRegistry.SoftDeletedSessionGauge.Inc();
        //        _softDeletedSessions++;
        //    }
        //}
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
        try
        {
            var pending = GetPendingHunt(sessionId);
            pending.Activate();
            Log.Information("Activated PendingHunt with sessionID {sessionId}", sessionId);
        }
        catch (Exception e)
        {
            Log.Warning(e, $"Function: ActivateSession ID:{sessionId}");
            throw;
        }
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
    public void SetCourse(string sessionId, entities.HydratedCourse course)
    {
        Log.Information("Starting setting Course for Session with ID {session} and Course {var}", sessionId, course);
        var pending = GetPendingHunt(sessionId);
        // retrieve course by GUId from db
        pending.Settings.SelectedCourse = course;
        Log.Information("Completed setting Course for Session with ID {session} and Course {var}", sessionId, course);
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
        ArgumentOutOfRangeException.ThrowIfInvalidEnum<ShotType, int>(
            scoringVariant,
            nameof(scoringVariant)
        );
        Log.Information("Starting setting Shotvariant for Session with ID {session} and Variant {var}", sessionId, scoringVariant);
        var pending = GetPendingHunt(sessionId);

        pending.Settings.ScoringVariant = (ShotType)scoringVariant;
        Log.Information("Completed setting Shotvariant for Session with ID {session} and Variant {var}", sessionId, scoringVariant);
    }

    public bool PlayerJoined(string sessionId, Guid playerId)
    {
        var session = GetSession(sessionId);
        if (session.IsHunt())
        {
            session.Hunt.Players.Add(playerId);
            return true;
        }
        if (session.IsPending())
        {
            session.Pending.Players.Add(playerId);
            return true;
        }
        return false;
    }

    public bool PlayerLeft(string sessionId, Guid playerId)
    {
        var session = GetSession(sessionId);
        if (session.IsHunt())
        {
            session.Hunt.Players.Remove(playerId);
            return true;
        }
        if (session.IsPending())
        {
            session.Pending.Players.Remove(playerId);
            return true;
        }
        return false;
    }

    public entities.Shot RegisterShot(string sessionId, Guid playerId, Guid animalId, int points, int shotNumber)
    {
        var hunt = GetHunt(sessionId);
        return hunt.Scores.RegisterShot(playerId, animalId, points, shotNumber, hunt.UUID);
    }

    public AllStats GetStats(string sessionId)
    {
        var hunt = GetHunt(sessionId);
        var ranks = hunt.Scores.GetRanking();
        if (ranks.Count == 0)
        {
            Log.Information("Ranks is empty");
        }
        return new AllStats(ranks);
    }

    public UserStats GetUserStats(string sessionId, Guid player)
    {
        var hunt = GetHunt(sessionId);
        var ranks = hunt.Scores.GetRanking();
        var shots = hunt.Scores.GetShotsForPlayer(player);
        if (shots.Count == 0)
        {
            Log.Information("Shots for Player {playerid} is empty", player);
        }

        var counterKillShot = 0;
        var counterHit = 0;
        var counterMiss = 0;
        foreach (var shot in shots)
        {
            if (shot.Score == 0)
            {
                counterMiss++;
            }
            if (shot.Score > 0)
            {
                counterHit++;
            }
            if (IsKillShot(shot))
            {
                counterKillShot++;
            }
        }
        int? playerRank = ranks?.FirstOrDefault(kvp => kvp.Key == player).Value;
        int rank = -1;
        if (playerRank is not null)
        {
            rank = playerRank.Value;
        }
        return new UserStats(player, counterKillShot, counterHit, counterMiss, rank);
    }

    private static bool IsKillShot(entities.Shot shot)
    {
        if (shot.Score == 0)
        {
            return false;
        }

        // Zweipfeil 20 Score immer Kill
        // Dreipfeil Erster Pfeil 20 Kill
        // Dreipfeil Zweiter 16 Kill
        // Dreipfeil Dritter 10 Kill
        return shot switch
        {
            // Zweipfeil: always kill on 20
            { Kind: 2, Score: 20 } => true,

            // Dreipfeil rules
            { Kind: 3, ShotNumber: 1, Score: 20 } => true,
            { Kind: 3, ShotNumber: 2, Score: 16 } => true,
            { Kind: 3, ShotNumber: 3, Score: 10 } => true,

            // everything else
            _ => false
        };
    }

    public void RemovePlayerFromSessions(Guid playerId)
    {
        foreach (var hunt in _hunts)
        {
            var val = hunt.Value;
            var session = val.Value;
            if (session is null)
            {
                continue;
            }
            session.Players.Remove(playerId);
        }
        foreach (var hunt in _pendingHunts)
        {
            var val = hunt.Value;
            var session = val.Value;
            if (session is null)
            {
                continue;
            }
            session.Players.Remove(playerId);
        }
    }


    public SessionReference GetSession(string sessionId)
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
        var now = DateTime.UtcNow;
        long cleanUpCount = 0;
        Log.Information("Starting CleanUp of Soft Deleted Sessions at {currentUTCTime}", now);

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
            cleanUpCount = huntsRemoved = pendingRemoved;
            _softDeletedSessions -= cleanUpCount;
            MetricsRegistry.SoftDeletedSessionGauge.Dec(cleanUpCount);
        }
        Log.Information("Completed CleanUp Run started at {startTimeUTC}, removed {amount} Soft Deleted Sessions", now, cleanUpCount);
    }

    public void StopCleanUp()
    {
        Log.Information("CleanUp is being stopped TimeStamp {time}", DateTime.UtcNow);
        _cleanupTimer?.Dispose();
    }

    public void Dispose()
    {
        Log.Information("CleanUp is being stopped TimeStamp {time}", DateTime.UtcNow);
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
                if (IsDeleted())
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
        private readonly DateTime _createdAt;
        private static readonly TimeSpan _expirationDuration = TimeSpan.FromDays(3);
        private bool _isDeleted;
        [MemberNotNullWhen(false, nameof(Value))]
        public bool IsDeleted()
        {
            //if (IsExpired())
            //{
            //    return true;
            //}
            return _isDeleted;
        }
        private bool IsExpired()
        {
            return DateTime.UtcNow > _createdAt + _expirationDuration;
        }

        public SessionEntry(T value)
        {
            _val = value ?? throw new ArgumentNullException(nameof(value));
            _isDeleted = false;
            _createdAt = DateTime.UtcNow;
        }

        public void SoftDelete()
        {
            _isDeleted = true;
        }
    }

    public class SessionReference
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

public record AllStats(List<KeyValuePair<Guid, int>> Ranking);
public record UserStats(Guid User, int Kill, int Hit, int Miss, int Rank);

public sealed class SessionNotFoundException : Exception, IApiErrorConvertible, IDetailProvider
{
    public IDictionary<string, object?> Details { get; init; } = new Dictionary<string, object?>();
    public string SessionId { get; }

    public SessionNotFoundException(string sessionId)
        : base($"Session '{sessionId}' does not exist.")
    {
        SessionId = sessionId;
        Details.Add("session_id", SessionId);
    }

    public ApiError ToApiError()
    {
        var result = new ApiError("session_not_found", "The requested session could not be found");
        result.MergeDetails(this);
        return result;
    }
}

public sealed class SessionDeletedException : Exception, IApiErrorConvertible, IDetailProvider
{
    public IDictionary<string, object?> Details { get; init; } = new Dictionary<string, object?>();
    public string SessionId { get; }

    public SessionDeletedException(string sessionId)
        : base($"Session '{sessionId}' has been deleted.")
    {
        SessionId = sessionId;
        Details.Add("session_id", SessionId);
    }

    public ApiError ToApiError()
    {
        var result = new ApiError("session_deleted", "The requested session has been deleted");
        result.MergeDetails(this);
        return result;
    }
}