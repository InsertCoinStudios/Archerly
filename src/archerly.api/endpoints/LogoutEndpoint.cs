namespace archerly.api.endpoints;

public static class LogoutEndpoint
{
    public static void MapLogoutEndpoints(this IEndpointRouteBuilder app)
    {
        // Contract: Empty or ApiError
        app.MapPost("/logout", PostLogout);
    }
    private static IResult PostLogout()
    {
        // TODO: Register
        return Results.Ok();
    }
}