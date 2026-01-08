using archerly.core.extensions;
namespace archerly.core.hunts;

public class PlayerList
{
    private readonly List<Guid> _players = new();
    private readonly Lock _playerLock = new();

    public Guid Owner { get; private set; }

    public Action RequestDissolution { get; set; } = () => { };
    public Func<PlayerList, Guid> OwnerShipTransferStrategy { get; set; }

    public IReadOnlyList<Guid> ToList
    {
        get
        {
            lock (_playerLock)
            {
                return _players.ToList();
            }
        }
    }

    public PlayerList(Guid owner, Func<PlayerList, Guid> transferFunc)
    {
        ArgumentNullException.ThrowIfNull(owner);
        ArgumentNullException.ThrowIfNull(transferFunc);
        OwnerShipTransferStrategy = transferFunc;

        Owner = owner;
        _players.Add(owner);
    }

    public void Add(Guid player)
    {
        ArgumentNullException.ThrowIfNull(player);

        lock (_playerLock)
        {
            _players.Add(player);
        }
    }

    public void Remove(Guid leavingPlayer)
    {
        ArgumentException.ThrowIfEmpty(leavingPlayer);

        SessionAction action = GetActionAfterLeave(leavingPlayer);

        lock (_playerLock)
        {
            _players.Remove(leavingPlayer);
        }

        switch (action)
        {
            case SessionAction.TransferOwnership:
                TransferOwnership();
                break;
            case SessionAction.Dissolve:
                RequestDissolution();
                break;
            case SessionAction.Persist:
            default:
                break;
        }
    }

    private SessionAction GetActionAfterLeave(Guid leavingPlayer)
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
            Guid nextOwner = OwnerShipTransferStrategy(this);
            if (!nextOwner.Equals(Guid.Empty))
            {
                Owner = nextOwner;
            }
        }
    }

    /// <summary>
    /// Represents a method that handles a <see cref="Guid"/>.
    /// </summary>
    public delegate void HandleGuid(Guid id);
}
