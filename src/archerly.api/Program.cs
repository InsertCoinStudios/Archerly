namespace archerly.api;

using Serilog;
using Serilog.Sinks.Loki;
using archerly.api.extensions;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        //Setup Loki config either with user credntials or without
        // Address to local or remote Loki server
        var credentials = new BasicAuthCredentials("http://localhost:3100", "admin", "admin");
        //var credentials = new NoAuthCredentials("http://localhost:3100"); 

        Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Information()
                    .Enrich.FromLogContext()
                    .WriteTo.LokiHttp(credentials)
                    .CreateLogger();

        var app = builder.Build();

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
