namespace archerly.core.hunts;

public class PendingHunt
{
    public string SessionId { get; }
    public PlayerList Players { get; }
    public PendingHuntSettings Settings { get; }

    private readonly Action<Hunt> _transitionAction;
    private readonly Lock _activateLock = new();
    private bool _activated;

    public PendingHunt(Guid owner, Action<Hunt> transitionAction, Func<PlayerList, Guid> transferFunc)
    {
        ArgumentNullException.ThrowIfNull(transitionAction);

        SessionId = SessionIdGeneratorSingleton.Instance.Next();
        Players = new PlayerList(owner, transferFunc);
        Settings = new PendingHuntSettings();
        _transitionAction = transitionAction;
    }
    /// <summary>
    /// Activates this pending hunt and transitions it into an active <see cref="Hunt"/>.
    /// </summary>
    /// <remarks>
    /// If this pending hunt has not yet been activated, this method will create a <see cref="Hunt"/> instance
    /// based on the current settings and invoke the transition action.  
    /// Thread safety is ensured during the activation process.
    /// </remarks>
    /// <exception cref="HuntAlreadyActivatedException">
    /// Thrown if this pending hunt has already been activated.
    /// </exception>
    /// <exception cref="ScoringVariantNotSetException">
    /// Thrown if the scoring variant has not been set in the pending hunt settings.
    /// </exception>
    /// <exception cref="CourseNotSetException">
    /// Thrown if the selected course has not been set in the pending hunt settings.
    /// </exception>
    public void Activate()
    {
        lock (_activateLock)
        {
            if (_activated)
            {
                throw new HuntAlreadyActivatedException();
            }
            //throws ScoringVariantNotSetException;
            //throws CourseNotSetException;
            HuntSettings settings = Settings.Build();
            Hunt hunt = new Hunt(settings, this);

            _activated = true;
            _transitionAction(hunt);
        }
    }
}
public class HuntAlreadyActivatedException : Exception
{
    public HuntAlreadyActivatedException()
        : base("PendingHunt has already been finalized.")
    {
    }
}