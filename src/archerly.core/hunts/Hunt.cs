using System.Diagnostics.CodeAnalysis;

namespace archerly.core.hunts;

public class Hunt
{
    private readonly HuntManager _manager;
    private readonly ScoreBoard _scores;
    private readonly Course _course;
    private readonly ShotType _scoringVariant;

    public User Owner { get; private set; }
    private readonly List<User> _players;
    public IReadOnlyList<User> Players => _players.AsReadOnly();

    /// <summary>
    /// Function that decides the next owner of a Hunt when the current owner leaves.
    /// Returns a User to transfer ownership to.
    /// </summary>
    public delegate User? OwnershipTransferStrategy(IReadOnlyList<User> remainingPlayers);
    private OwnershipTransferStrategy _ownershipTransferStrategy { get; set; }

    private readonly Lock _lock = new();

    public Hunt(User Owner, ShotType scoringVariant, Course course, HuntManager manager)
    {
        _scoringVariant = scoringVariant;
        _scores = new ScoreBoard(scoringVariant);
        _course = course;
        _players = new();
        this.Owner = Owner;
        _manager = manager;
        // Toy
        _ownershipTransferStrategy = Players => TransferStrategies.TransferToTop(Players);
    }

    public void AddUser(Guid Id)
    {
        // TODO: Create User from Guid
        var player = new User(Id);
        AddUser(player);
    }

    public void AddUser(User player)
    {
        lock (_lock)
        {
            _players.Add(player);
        }
    }

    public void RemoveUser(Guid id)
    {
        var leavingPlayer = new User(id);
        RemoveUser(leavingPlayer);
    }

    public void RemoveUser(User leavingPlayer)
    {
        var action = GetActionAfterLeave(leavingPlayer);
        lock (_lock)
        {
            _players.Remove(leavingPlayer);
        }
        switch (action)
        {
            case SessionAction.TransferOwnership:
                TransferOwnershipAction();
                break;
            case SessionAction.Dissolve:
                DissolveAction();
                break;
            default:
                break;
        }
    }

    private SessionAction GetActionAfterLeave(User leavingPlayer)
    {
        if (leavingPlayer.Equals(Owner))
        {
            return SessionAction.TransferOwnership;
        }

        int remainingPlayers = Players.Count - 1;
        if (remainingPlayers == 0)
        {
            return SessionAction.Dissolve;
        }
        else
        {
            return SessionAction.Persist;
        }
    }

    private void TransferOwnershipAction()
    {
        var next = _ownershipTransferStrategy(Players);
        if (next is not null)
        {
            Owner = next;
        }
    }

    private void DissolveAction()
    {
        _manager.RequestDissolve(this);
    }

    public void RegisterShot(User Player, Animal Target, long Points)
    {
        _scores.RegisterShot(Player, Target, Points);
    }

    private class TransferStrategies
    {

        public static User? DissolveOnOwnerLeave(IReadOnlyList<User> remainingPlayers, Hunt hunt)
        {
            hunt._manager.RequestDissolve(hunt);
            return null;
        }

        public static User? TransferToTop(IReadOnlyList<User> remainingPlayers)
        {
            return remainingPlayers.FirstOrDefault();
        }

        public static User? TransferBasedOnRanking(IReadOnlyList<User> remainingPlayers, ScoreBoard _scores)
        {
            var scores = _scores.GetRanking();
            foreach (var kv in scores)
            {
                if (remainingPlayers.Contains(kv.Key))
                {
                    return kv.Key;
                }
            }
            return null;
        }
    }
}
