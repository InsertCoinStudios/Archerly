
namespace archerly.api.endpoints;

public static class RegisterEndpoint
{
    public static void MapRegisterEndpoints(this IEndpointRouteBuilder app)
    {
        // Contract: JWT or ApiError
        app.MapPost("/register", PostRegister);
    }
    private static IResult PostRegister()
    {
        // TODO: Register
        return Results.Ok();
    }
}