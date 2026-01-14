using System.Threading.Tasks;

namespace archerly.api.endpoints;

public static class LoginEndpoint
{
    public static void MapLoginEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/login", PostLogin);
    }

    private static async Task<IResult> PostLogin(LoginRequest request, Supabase.Client client)
    {
        var session = await client.Auth.SignIn(request.Email, request.Password);
        if (session is null)
        {
            return Results.Unauthorized();
        }
        var jwt = session.AccessToken;
        if (jwt is null)
        {
            return Results.Unauthorized();
        }
        // TODO: Query Database for if the UserId is Admin
        bool isAdmin = false;
        var response = new LoginResponse(
            jwt,
            isAdmin,
            new ExpirationData(session.ExpiresIn, session.ExpiresAt())
            );
        return Results.Ok(response);
    }
}
public record LoginRequest(string Email, string Password);
public record LoginResponse(string JWT, bool IsAdmin, ExpirationData Expiration);
public record ExpirationData(long ExpiresInSec, DateTime ExpiresAt);
