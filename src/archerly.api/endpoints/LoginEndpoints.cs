using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using archerly.api.helpers;
using archerly.core;
using archerly.database.repos;
using archerly.database.repos.interfaces;
using Serilog;

namespace archerly.api.endpoints;

public static class LoginEndpoint
{
    public static void MapLoginEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/login", PostLogin);
    }

    private static async Task<IResult> PostLogin(LoginRequest request, IUserRepository repo, Supabase.Client client)
    {
        var session = await client.Auth.SignIn(request.Email, request.Password);
        if (session is null)
        {
            Log.Error("Endpoint Login: Session is null Path");
            return Results.Unauthorized();
        }
        var jwt = session.AccessToken;
        if (jwt is null)
        {
            Log.Error("Endpoint Login: JWT is null Path");
            return Results.Unauthorized();
        }

        if (!JwtHelpers.TryGetUserGuidFromRawToken("Login", jwt, out Guid user_id, out IResult? error))
        {
            Log.Error($"Endpoint Login: Actuator: {nameof(JwtHelpers.TryGetUserGuidFromRawToken)}, {error.ToString()}");
            return error;
        }

        var user = await repo.GetByIdAsync(user_id);
        if (user is null)
        {
            Log.Error($"Endpoint Login: Retrieved User is null");
            return Results.Problem(new ApiError(
                "retrieved_jwt_but_no_user_found",
                "Supabase returned a JWT but there is no user for this saved")
                .ToString(),
                statusCode: 500,
                type: "login:failed",
                title: "retrieved_jwt_but_no_user_found"
                );
        }
        bool isAdmin = user.IsAdmin;
        var response = new LoginResponse(
            user_id.ToString(),
            jwt,
            isAdmin,
            new ExpirationData(session.ExpiresIn, session.ExpiresAt())
            );
        return Results.Ok(response);
    }
}
public record LoginRequest(string Email, string Password);
public record LoginResponse(string UserId, string JWT, bool IsAdmin, ExpirationData Expiration);
public record ExpirationData(long ExpiresInSec, DateTime ExpiresAt);
