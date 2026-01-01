using archerly.core.extensions;
namespace archerly.core.hunts;

public class SessionManager
{
    private readonly Dictionary<string, SessionEntry<Hunt>> _hunts = new();
    private readonly Dictionary<string, SessionEntry<PendingHunt>> _pendingHunts = new();
    private readonly Lock _lock = new();

    public void AddHunt(Hunt hunt)
    {
        ArgumentNullException.ThrowIfNull(hunt);

        lock (_lock)
        {
            _hunts[hunt.SessionId] = new SessionEntry<Hunt>(hunt);
        }
    }

    public void AddPendingHunt(PendingHunt pendingHunt)
    {
        ArgumentNullException.ThrowIfNull(pendingHunt);

        lock (_lock)
        {
            _pendingHunts[pendingHunt.SessionId] = new SessionEntry<PendingHunt>(pendingHunt);
        }
    }

    public void RemoveHunt(string sessionId)
    {
        lock (_lock)
        {
            if (_hunts.TryGetValue(sessionId, out var entry))
            {
                entry.SoftDelete();
            }
        }
    }

    public void RemovePendingHunt(string sessionId)
    {
        lock (_lock)
        {
            if (_pendingHunts.TryGetValue(sessionId, out var entry))
            {
                entry.SoftDelete();
            }
        }
    }

    public void Remove(string sessionId)
    {
        lock (_lock)
        {
            if (_hunts.TryGetValue(sessionId, out var hunt))
            {
                hunt.SoftDelete();
            }
            if (_pendingHunts.TryGetValue(sessionId, out var pendingHunt))
            {
                pendingHunt.SoftDelete();
            }
        }
    }

    public void TransitionFromPending(Hunt hunt)
    {
        RemovePendingHunt(hunt.SessionId);
        AddHunt(hunt);
    }

    public void SetCourse(string sessionId, Guid courseId)
    {
        PendingHunt target;
        lock (_lock)
        {
            // Check if there is a pending hunt with the sessionId
            if (!_pendingHunts.TryGetValue(sessionId, out var entry))
            {
                throw new SessionNotFoundException(sessionId);
            }
            if (entry.IsDeleted)
            {
                throw new SessionDeletedException(sessionId);
            }
            target = entry.Value ?? throw new ArgumentNullException(nameof(entry.Value)); // this should never happen log this
        }
        // retrieve course by GUId from db
        // TODO: Replace with call to the repository
        var course = new Course(courseId);
        target.Settings.SelectedCourse = course;
    }

    public void SetScoringVariant(string sessionId, int scoringVariant)
    {
        PendingHunt target;
        ArgumentOutOfRangeException.ThrowIfInvalidEnum<ShotType, int>(scoringVariant, nameof(scoringVariant));
        lock (_lock)
        {
            // Check if there is a pending hunt with the sessionId
            if (!_pendingHunts.TryGetValue(sessionId, out var entry))
            {
                throw new SessionNotFoundException(sessionId);
            }
            if (entry.IsDeleted)
            {
                throw new SessionDeletedException(sessionId);
            }
            target = entry.Value ?? throw new ArgumentNullException(nameof(entry.Value)); // this should never happen log this
        }
        if (!scoringVariant.TryToEnum(out ShotType variant))
        {
            // log this the exception did not get thrown
        }
        target.Settings.ScoringVariant = variant;
    }

    // Garbage Collector Function
    public void Cleanup()
    {
        lock (_lock)
        {
            _hunts.RemoveAll(kvp => kvp.Value.IsDeleted);
            _pendingHunts.RemoveAll(kvp => kvp.Value.IsDeleted);
        }
    }


    internal class SessionEntry<T>
    {
        public T? Value
        {
            get
            {
                if (IsDeleted)
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
        public bool IsDeleted { get; private set; }

        public SessionEntry(T value)
        {
            _val = value ?? throw new ArgumentNullException(nameof(value));
            IsDeleted = false;
        }

        public void SoftDelete()
        {
            IsDeleted = true;
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