using archerly.api.endpoints;
namespace archerly.api.extensions;

using Prometheus;

public static class WebAppBuilderExtensions
{
    public static IApplicationBuilder UseMetrics(this IApplicationBuilder self)
    {
        self.UseMetricServer();
        self.UseHttpMetrics();
        return self;
    }

    public static IApplicationBuilder UseRoutes(this WebApplication self)
    {
        // Activate Login
        self.MapLoginEndpoints();
        // Activate Register Endpoint
        self.MapRegisterEndpoints();
        // Activate Hunt
        self.MapHuntEndpoints();
        // Activate All Time Statistic
        self.MapAllTimeStatEndpoints();
        // Activate Course Endpoint
        self.MapCourseEndpoints();
        // Activate Animal Endpoints
        self.MapAnimalEndpoints();
        // Activate Image Endpoints
        self.MapImageEndpoints();


        var summaries = new[] { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" };

        self.MapGet("/weatherforecast", () =>
        {
            var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
                .ToArray();
            return forecast;
        })
        .WithName("GetWeatherForecast");
        return self;
    }
}