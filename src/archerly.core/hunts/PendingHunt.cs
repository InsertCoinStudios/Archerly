namespace archerly.core.hunts;

public class PendingHunt
{
    public string SessionId { get; }
    public PlayerList Players { get; }
    public PendingHuntSettings Settings { get; }

    private readonly Action<Hunt> _onFinalized;
    private readonly Lock _activateLock = new();
    private bool _activated;

    public PendingHunt(Guid ownerId, Action<Hunt> onFinalized, Func<PlayerList, User?> transferFunc)
    {
        ArgumentNullException.ThrowIfNull(onFinalized);
        var owner = new User(ownerId);

        SessionId = SessionIdGeneratorSingleton.Instance.Next();
        Players = new PlayerList(owner, transferFunc);
        Settings = new PendingHuntSettings();
        _onFinalized = onFinalized;
    }

    public void Activate()
    {
        lock (_activateLock)
        {
            if (_activated)
            {
                throw new HuntAlreadyFinalizedException();
            }
            HuntSettings settings = Settings.Build();
            Hunt hunt = new Hunt(settings, this);

            _activated = true;
            _onFinalized(hunt);
        }
    }
}
public class HuntAlreadyFinalizedException : Exception
{
    public HuntAlreadyFinalizedException()
        : base("PendingHunt has already been finalized.")
    {
    }
}