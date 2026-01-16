using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using archerly.api.helpers;
using archerly.core;
using archerly.database.repos;

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

        if (!JwtHelpers.TryGetUserGuidFromRawToken("Login", jwt, out Guid user_id, out IResult? error))
        {
            return error;
        }

        var user = await new SupaBaseUserRepo(client).GetByUserIdlAsync(user_id);
        if (user is null)
        {
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
