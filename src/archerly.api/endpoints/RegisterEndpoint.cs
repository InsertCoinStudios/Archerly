
using System.Text.Json;
using archerly.core;
using archerly.database.repos;
using archerly.database.repos.interfaces;
using Serilog;

namespace archerly.api.endpoints;

// TODO: In Supabase set Confirm Email and Confirm Phonenumber to off
// if this is not done it will be a lot harder for the backend
public static class RegisterEndpoint
{
    public static void MapRegisterEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/register", PostRegister);
    }
    private async static Task<IResult> PostRegister(RegisterRequest request, Supabase.Client client, IUserRepository repo)
    {
        var session = await client.Auth.SignUp(request.Email, request.Password);
        if (session is null)
        {
            return Results.Problem(
                title: "Registration failed",
                detail: "No response from authentication provider",
                statusCode: StatusCodes.Status501NotImplemented
            );
        }
        if (session.User == null || string.IsNullOrEmpty(session.AccessToken) || session.User.Id is null)
        {
            return Results.Problem(
                title: "user_registration_failed_or_is_already_registered",
                detail: "No response from authentication provider",
                statusCode: StatusCodes.Status502BadGateway
            );
        }
        if (!Guid.TryParse(session.User.Id, out Guid guid))
        {
            return Results.Problem(
                title: "User Guid Parse failed",
                detail: $"Could not Parse Guid Primitive: {session.User.Id}",
                statusCode: StatusCodes.Status503ServiceUnavailable
            );
        }
        // 🧱 Create domain user
        var user = entities.User.NewUserWithId(
            guid,
            request.FirstName,
            request.LastName,
            request.Nickname,
            isAdmin: false
        );
        Log.Information("Guid: {guid} FirstName: {firstname} LastName: {lastname} Nickname: {nickname} IsAdmin {isadmin}", guid, request.FirstName, request.LastName, request.Nickname, false);
        try
        {
            if (user.Id == Guid.Empty)
            {
                Log.Error("The User ID is {id}", user.Id);
                return Results.Problem(
                    title: "User Guid Is still Empty or Null",
                    detail: $"Guid is Null: {user.Id}",
                    statusCode: StatusCodes.Status507InsufficientStorage
                );
            }
            await repo.AddAsync(user);
        }
        catch (Exception e)
        {
            Log.Warning(e, $"Function: {nameof(PostRegister)}");
            return Results.InternalServerError();
        }

        return Results.Ok();
    }
}

public record RegisterRequest(string Email, string Password, string Nickname, string FirstName, string LastName);
public record RegisterResponse(bool Success, ApiError? Error);