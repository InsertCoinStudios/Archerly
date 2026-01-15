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
        if (JwtHelpers.TryGetUserGuidFromClaim("Logout", user, out Guid guid, out IResult? error))
        {
            // loops over all sessions and removes that guid
            manager.RemoveUserFromSessions(guid);
            return Results.Ok();
        }
        else
        {
            return error;
        }
    }
}