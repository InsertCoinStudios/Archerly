using System.Security.Claims;
using archerly.api.helpers;

namespace archerly.api.endpoints;

public static class AllTimeStatsEndpoint
{

    public static void MapAllTimeStatEndpoints(this IEndpointRouteBuilder app)
    {
        // Contract DTO of AllTimeStats or APIError DTO
        app.MapPost("/allTimeStats", GetAllTimeStats);
    }

    private static IResult GetAllTimeStats(ClaimsPrincipal user)
    {
        if (!JwtHelpers.TryGetUserGuidFromClaim(nameof(GetAllTimeStats), user, out Guid guid, out IResult? error))
        {
            return error;
        }
        // TODO: AllTimeStats
        // TODO: GetAllTimeStats from DB
        // TODO: GetAll Shots the User ever made
        return Results.Ok();
    }
}