namespace archerly.core.hunts;

public sealed class HuntManagerSingleton : IDisposable
{
    private static readonly Lazy<HuntManagerSingleton> _lazy =
        new Lazy<HuntManagerSingleton>(() => new HuntManagerSingleton(true, 5));

    public static HuntManagerSingleton Instance => _lazy.Value;

    // The actual HuntManager
    public HuntManager Ressource { get; }

    private bool _disposed;

    // Private constructor ensures singleton
    private HuntManagerSingleton(bool autoCleanup, long cleanupIntervalInMinutes)
    {
        Ressource = new HuntManager(autoCleanup, cleanupIntervalInMinutes);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                Ressource.Dispose();
            }
            _disposed = true;
        }
    }
}
