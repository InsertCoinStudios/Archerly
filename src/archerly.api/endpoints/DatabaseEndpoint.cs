using archerly.database.jsondb;

namespace archerly.api.endpoints;

public static class DbEndpoint
{
    public static void MapDatabaseDebugEndpoints(this IEndpointRouteBuilder app)
    {
        // GET /debug/db - returns the full JSON database
        app.MapGet("/debug/db", async (JsonDatabaseStore store) =>
        {
            var db = store.Load();

            // Serialize with indentation for readability
            var options = new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true
            };

            var json = System.Text.Json.JsonSerializer.Serialize(db, options);
            return Results.Content(json, "application/json");
        });
    }
}