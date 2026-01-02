namespace archerly.core.hunts;

public sealed class SessionIdGeneratorSingleton
{
    private static readonly Lazy<SessionIdGeneratorSingleton> _instance =
        new Lazy<SessionIdGeneratorSingleton>(() => new SessionIdGeneratorSingleton());

    public static SessionIdGeneratorSingleton Instance => _instance.Value;

    public SessionIdGenerator Ressource { get; }

    // Private constructor for singleton
    private SessionIdGeneratorSingleton()
    {
        Ressource = new SessionIdGenerator();
    }
}