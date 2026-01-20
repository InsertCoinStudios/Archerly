namespace archerly.api;

using Serilog;
using Serilog.Sinks.Loki;
using archerly.api.extensions;
using archerly.metrics;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.AddSupabase();
        builder.AddHuntManagerService();
        builder.AddRepoServices();

        builder.Services.AddAuthorization();

        // Add services to the container.
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        //Setup Loki config either with user credntials or without
        // Address to local or remote Loki server
        //var credentials = new BasicAuthCredentials("http://loki:3100", "admin", "admin");
        //var credentials = new NoAuthCredentials("http://loki:3100");

        Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Information()
                    .Enrich.FromLogContext()
                    //.WriteTo.LokiHttp(credentials)
                    .WriteTo.File(
                        path: "/app/logs/archerly.log",        // path inside container
                        rollingInterval: RollingInterval.Day,  // one file per day
                        retainedFileCountLimit: 7,             // keep last 7 days
                        outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"
                    )
                    .CreateLogger();
        Log.Information("Logger Configured");

        var app = builder.Build();

        app.UseAuthentication();
        app.UseAuthorization();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        // set up metrics instrumentation
        app.UseMetrics();

        // define all api routes
        app.UseRoutes();

        app.Run();
    }
}

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
