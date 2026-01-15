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
        if (!JwtHelpers.GetUserId(user, out string? userId))
        {
            return Results.Unauthorized();
        }
        // TODO: AllTimeStats
        // TODO: GetAllTimeStats from DB
        // Use JWT to get the AllTimeStats for the user
        return Results.Ok();
    }
}