
using archerly.core;

namespace archerly.api.endpoints;

// TODO: In Supabase set Confirm Email and Confirm Phonenumber to off
// if this is not done it will be a lot harder for the backend
public static class RegisterEndpoint
{
    public static void MapRegisterEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/register", PostRegister);
    }
    private async static Task<IResult> PostRegister(RegisterRequest request, Supabase.Client client)
    {
        try
        {
            var session = await client.Auth.SignUp(request.Email, request.Password);
            if (session is null)
            {
                return Results.Problem(
                    title: "Registration failed",
                    detail: "No response from authentication provider",
                    statusCode: StatusCodes.Status502BadGateway
                );
            }
            if (session.User == null || string.IsNullOrEmpty(session.AccessToken))
            {
                return Results.Conflict(new RegisterResponse(
                    Success: false,
                    Error: new ApiError(
                        "user_registration_failed_or_is_already_registered",
                        "The registration failed since Supabase returned invalid data")
                ));
            }
            return Results.Ok();
        }
        catch (Exception ex)
        {
            return Results.Problem(
                title: "Registration failed",
                detail: ex.Message,
                statusCode: StatusCodes.Status400BadRequest
            );
        }
    }
}

public record RegisterRequest(string Email, string Password);
public record RegisterResponse(bool Success, ApiError? Error);