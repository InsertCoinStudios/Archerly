namespace archerly.core.hunts;

public class HuntManager
{
    // TODO: Implement this
    private readonly SessionManager _sessions = new();

    public HuntManager()
    {
    }

    public void CreateNewPendingHunt(Guid ownerId)
    {
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

    private class TransferStrategies
    {

        public static User? DissolveOnOwnerLeave(PlayerList players)
        {
            players.RequestDissolution();
            return null;
        }

        public static User? TransferToTop(PlayerList players)
        {
            return players.ToList.FirstOrDefault();
        }
    }
}