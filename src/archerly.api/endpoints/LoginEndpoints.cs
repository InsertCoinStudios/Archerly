namespace archerly.api.endpoints;

public static class LoginEndpoint
{
    public static void MapLoginEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/login", PostLogin);
    }

    private static IResult PostLogin()
    {
        // TODO: Login
        // Retrieve credentials
        // Provide Credentials to Supabase
        // Get Supabase Response
        // Return result to caller
        return Results.Ok();
    }
}