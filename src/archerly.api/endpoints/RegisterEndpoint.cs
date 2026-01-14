
namespace archerly.api.endpoints;

// TODO: In Supabase set Confirm Email and Confirm Phonenumber to off
// if this is not done it will be a lot harder for the backend
public static class RegisterEndpoint
{
    public static void MapRegisterEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/register", PostRegister);
    }
    private static IResult PostRegister(RegisterRequest request, Supabase.Client client)
    {
        // TODO: Register
        return Results.Ok();
    }
}

public record RegisterRequest(string Email, string Password);
public record RegisterResponse(bool Success, string? ErrorMessage);