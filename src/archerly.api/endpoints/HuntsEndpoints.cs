
using System.Security.Claims;

namespace archerly.api.endpoints;

public static class HuntsEndPoint
{
    public static void MapHuntEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/hunts", GetHunts);
        app.MapGet("/hunts/{id}", GetHuntById);
        app.MapPost("/hunts", PostHunt);
        app.MapDelete("/hunts/{id}", DeleteHuntById);
        app.MapPatch("/hunts/{id}", PatchHuntById);
        app.MapPut("/hunts/{id}", PutHuntById);
        app.MapPost("/hunts/{id}/join", PostHuntJoinById);
        app.MapPost("/hunts/{id}/leave", PostHuntLeaveById);

        app.MapPost("/hunts/{huntId}/animals/{animalId}", PostHuntShotOnTargetByIds);
    }

    private static IResult GetHunts()
    {
        return Results.Ok();
    }
    private static IResult GetHuntById(string? id)
    {
        return Results.Ok(id);
    }

    /// <summary>
    /// Create a new Hunt from Scratch
    /// Returns a Session Id to reference the Hunt in the future
    /// </summary>
    /// <returns></returns>
    private static IResult PostHunt(ClaimsPrincipal user)
    {
        // TODO: Requires user to be logged in
        // Returns a new Session id for the hunt
        return Results.Ok();
    }

    private static IResult DeleteHuntById(string? id, ClaimsPrincipal user)
    {
        // TODO: Deleting a Hunt
        // Validate that the sender is the current owner of the hunt or an admin
        // If Not return UnAuthorized
        // If yes delete the hunt from memory
        return Results.Ok(id);
    }

    private static IResult PutHuntById(string? id, ClaimsPrincipal user)
    {
        // TODO: Replacing a Hunt
        // Validate that the sender is the current owner of the hunt or an admin
        // If Not return UnAuthorized
        return Results.Ok(id);
    }

    private static IResult PatchHuntById(string? id, ClaimsPrincipal user)
    {
        // TODO: Partially Updating a Hunt
        // Validate that the sender is the current owner of the hunt or an admin
        // If Not return UnAuthorized
        return Results.Ok(id);
    }

    private static IResult PostHuntJoinById(string? id, ClaimsPrincipal user)
    {
        // TODO: Join a Hunt
        // Validate that the user is logged in
        // If Not return UnAuthorized
        return Results.Ok(id);
    }
    private static IResult PostHuntLeaveById(string? id, ClaimsPrincipal user)
    {
        // TODO: Leave a Hunt
        // Validate that the user is logged in and that he is part of the hunt
        // If Not return UnAuthorized
        return Results.Ok(id);
    }

    private static IResult PostHuntShotOnTargetByIds(string? huntId, string? animalId)
    {
        // TODO:
        // Uses the Huntid and the animalId and userId from the jwt to save a shot
        return Results.Ok($"{huntId} | {animalId}");
    }
}