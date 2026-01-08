namespace archerly.core.hunts;

public class HuntManager : IDisposable
{
    private readonly SessionManager _sessions;
    private const short _maxSessions = 4096;

    public HuntManager(bool cleanUp, long intervalInMinutes)
    {
        if (cleanUp)
        {
            _sessions = SessionManager.WithAutomaticCleanup(intervalInMinutes);
        }
        else
        {
            _sessions = new();
        }
    }

    public void CreateNewPendingHunt(Guid ownerId)
    {
        if (_sessions.Count > _maxSessions)
        {
            throw new InvalidOperationException(
                $"Cannot create a new pending hunt: maximum number of sessions ({_maxSessions}) reached."
            );
        }
        var transitionAction = _sessions.TransitionFromPending;
        var transferFunc = TransferStrategies.TransferToTop;
        PendingHunt pending = new(ownerId, transitionAction, transferFunc);

        var dissolveFunc = () => { _sessions.Remove(pending.SessionId); };
        pending.Players.RequestDissolution = dissolveFunc;
        _sessions.AddPendingHunt(pending);
    }

    public void SetCourseForPendingHunt(string sessionId, Guid courseId)
    {
        _sessions.SetCourse(sessionId, courseId);
    }

    public void SetScoringVariantForPendingHunt(string sessionId, int scoringVariant)
    {
        _sessions.SetScoringVariant(sessionId, scoringVariant);
    }

    public void ActivatePendingHunt(string sessionId)
    {
        _sessions.ActivateSession(sessionId);
    }

    public void RemoveSession(string sessionId)
    {
        _sessions.Remove(sessionId);
    }
    public void PlayerJoined(string sessionId, Guid playerId)
    {
        _sessions.PlayerJoined(sessionId, playerId);
    }

    public void PlayerLeft(string sessionId, Guid playerId)
    {
        _sessions.PlayerLeft(sessionId, playerId);
    }

    // TODO: Accept Shot Made Call to the Hunt
    public void SaveShot(string sessionId, Guid playerId, Guid animalId, long points)
    {
        _sessions.RegisterShot(sessionId, playerId, animalId, points);
    }

    public void Dispose()
    {
        _sessions.Dispose();
    }

    private class TransferStrategies
    {

        public static Guid DissolveOnOwnerLeave(PlayerList players)
        {
            players.RequestDissolution();
            return Guid.Empty;
        }

        public static Guid TransferToTop(PlayerList players)
        {
            return players.ToList.FirstOrDefault();
        }
    }
}