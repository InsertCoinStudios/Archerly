namespace archerly.core.hunts;

public sealed class SessionIdGeneratorSingleton
{
    private static readonly Lazy<SessionIdGeneratorSingleton> _instance =
        new Lazy<SessionIdGeneratorSingleton>(() => new SessionIdGeneratorSingleton());

    public static SessionIdGeneratorSingleton Instance => _instance.Value;

    private readonly SessionIdGenerator _generator;

    // Private constructor for singleton
    private SessionIdGeneratorSingleton()
    {
        _generator = new SessionIdGenerator();
    }

    /// <summary>
    /// Returns the next session ID.
    /// </summary>
    public string Next()
    {
        lock (_generator) // ensure thread safety
        {
            return _generator.Next();
        }
    }

    /// <summary>
    /// Optionally restart from a given last session ID.
    /// </summary>
    public void RestartFrom(string lastSessionId)
    {
        lock (_generator)
        {
            _generator.RestartFrom(lastSessionId);
        }
    }
}