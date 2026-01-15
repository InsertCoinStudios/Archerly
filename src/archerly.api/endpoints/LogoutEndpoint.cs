using System.Security.Claims;
using archerly.api.helpers;
using archerly.core;
using archerly.core.hunts;

namespace archerly.api.endpoints;

public static class LogoutEndpoint
{
    public static void MapLogoutEndpoints(this IEndpointRouteBuilder app)
    {
        // Contract: Empty or ApiError
        app.MapPost("/logout", PostLogout);
    }
    // Note: The Client promises to purge the JWT and to never use it again
    private static IResult PostLogout(ClaimsPrincipal user, HuntManager manager)
    {
        if (!JwtHelpers.TryGetUserId(user, out string? userId))
        {
            return Results.Problem(new ApiError(
                "failed_parsing_jwt",
                "Requester provided a JWT that could not be parsed")
                .ToString(),
                type: "logout:failed",
                title: "failed_parsing_jwt",
                statusCode: 500
                );
        }
        if (userId is null)
        {
            return Results.Problem(new ApiError(
                "failed_parsing_jwt",
                "Requester provided a JWT that could not be parsed")
                .ToString(),
                type: "logout:failed",
                title: "failed_parsing_jwt",
                statusCode: 500
                );
        }
        var guid = Guid.Parse(userId);
        // loops over all sessions and removes that guid
        manager.RemoveUserFromSessions(guid);
        return Results.Ok();
    }
}