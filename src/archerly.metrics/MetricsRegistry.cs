using Prometheus;

namespace archerly.metrics;

public static class MetricsRegistry
{
    public static readonly Gauge PlayerGauge = Metrics.CreateGauge(
        "player_gauge",
        "Tracks the currently Logged in Players"
        );

    public static readonly Gauge HuntGauge = Metrics.CreateGauge(
        "hunt_gauge",
        "Tracks the current number of Hunts (Sessions)"
        );
    public static readonly Gauge PendingHuntGauge = Metrics.CreateGauge(
        "pending_hunt_gauge",
        "Tracks the current number of Pending Hunts (not started Sessions)"
        );
    public static readonly Gauge SoftDeletedSessionGauge = Metrics.CreateGauge(
        "soft_deleted_session_gauge",
        "Tracks the number of currently marked as Soft Deleted Sessions"
        );
    public static readonly Counter GeneratedSessionIdsCounter = Metrics.CreateCounter(
        "generated_sessionID_counter",
        "Tracks the Number of generated Session IDs since startup"
        );
}

