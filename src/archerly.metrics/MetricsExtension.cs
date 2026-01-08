using Microsoft.AspNetCore.Builder;
using Prometheus;

namespace archerly.metrics;

/// <summary>
/// Provides an extension method to wire up Prometheus metrics in an ASP.NET project.
/// </summary>
public static class MetricsExtensions
{

    public static IApplicationBuilder UseMetrics(this IApplicationBuilder self)
    {
        {
            self.UseMetricServer();
            self.UseHttpMetrics();
            return self;
        }
    }
}