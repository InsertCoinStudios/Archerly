using Prometheus;

namespace archerly.metrics;

public static class MetricsRegistry
{
    public static readonly Counter ExampleCounter =
        Metrics.CreateCounter(
            "example_counter_total",
            "Counts example events"
        );

    public static readonly Gauge ExampleGauge =
        Metrics.CreateGauge(
            "example_gauge",
            "Tracks an example value"
        );

    public static readonly Histogram ExampleHistogram =
        Metrics.CreateHistogram(
            "example_histogram_seconds",
            "Tracks example durations in seconds"
        );
}

