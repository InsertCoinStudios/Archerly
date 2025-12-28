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

    // TODO: Accept setting config Calls to the hunt
    // TODO: Accept Activate Call to the Pending Hunt
    // TODO: Accept Shot Made Call to the Hunt
    // TODO: Accept Remove call to the Hunt
    // TODO: Accept Player Join Leave call to the Hunt


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