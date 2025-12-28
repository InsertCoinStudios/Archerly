namespace archerly.core.hunts;

public class PlayerList
{
    private readonly List<User> _players = new();
    private readonly Lock _playerLock = new();

    public User Owner { get; private set; }

    public Action RequestDissolution { get; set; } = () => { };
    public Func<PlayerList, User?> OwnerShipTransferStrategy { get; set; }

    public IReadOnlyList<User> ToList
    {
        get
        {
            lock (_playerLock)
            {
                return _players.ToList();
            }
        }
    }

    public PlayerList(User owner, Func<PlayerList, User?> transferFunc)
    {
        ArgumentNullException.ThrowIfNull(owner);
        ArgumentNullException.ThrowIfNull(transferFunc);
        OwnerShipTransferStrategy = transferFunc;

        Owner = owner;
        _players.Add(owner);
    }

    public void Add(User player)
    {
        ArgumentNullException.ThrowIfNull(player);

        lock (_playerLock)
        {
            _players.Add(player);
        }
    }

    public void Add(Guid id)
    {
        var player = new User(id);
        Add(player);
    }

    public void Remove(User leavingPlayer)
    {
        ArgumentNullException.ThrowIfNull(leavingPlayer);

        SessionAction action = GetActionAfterLeave(leavingPlayer);

        lock (_playerLock)
        {
            _players.Remove(leavingPlayer);
        }

        switch (action)
        {
            case SessionAction.TransferOwnership:
                {
                    TransferOwnership();
                    break;
                }
            case SessionAction.Dissolve:
                {
                    RequestDissolution();
                    break;
                }
            case SessionAction.Persist:
            default:
                {
                    break;
                }
        }
    }

    public void Remove(Guid id)
    {
        Remove(new User(id));
    }

    private SessionAction GetActionAfterLeave(User leavingPlayer)
    {
        if (leavingPlayer.Equals(Owner))
        {
            return SessionAction.TransferOwnership;
        }

        int remainingPlayers = _players.Count - 1;
        if (remainingPlayers == 0)
        {
            return SessionAction.Dissolve;
        }
        else
        {
            return SessionAction.Persist;
        }
    }

    private void TransferOwnership()
    {
        if (OwnerShipTransferStrategy != null)
        {
            User? nextOwner = OwnerShipTransferStrategy(this);
            if (nextOwner != null)
            {
                Owner = nextOwner;
            }
        }
    }
}
