
namespace archerly.api.endpoints;

public static class Hunts
{
    public static void MapHuntEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/hunts", GetHunts);
        app.MapGet("/hunts/{id}", GetHuntById);
        app.MapPost("/hunts", PostHunt);
        app.MapDelete("/hunts/{id}", DeleteHuntById);
        app.MapPatch("/hunts/{id}", PatchHuntById);
        app.MapPut("/hunts/{id}", PutHuntById);
    }

    private static IResult GetHunts()
    {
        return Results.Ok();
    }
    private static IResult GetHuntById(string? id)
    {
        return Results.Ok(id);
    }
    private static IResult PostHunt()
    {
        return Results.Ok();
    }

    private static IResult DeleteHuntById(string? id)
    {
        return Results.Ok(id);
    }

    private static IResult PutHuntById(string? id)
    {
        return Results.Ok(id);
    }

    private static IResult PatchHuntById(string? id)
    {
        return Results.Ok(id);
    }
}