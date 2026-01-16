using System.Security.Claims;
using archerly.api.helpers;
using archerly.database.repos;
using archerly.entities;
using Serilog;

namespace archerly.api.endpoints;

public static class AnimalEndpoints
{
    public static void MapAnimalEndpoints(this IEndpointRouteBuilder app)
    {
        // Contract: List<Animal> or ApiError
        app.MapGet("/animals", GetAnimals);
        // Contract: Animal or ApiError
        app.MapGet("/animals/{id}", GetAnimalById);
        // Contract: Empty or ApiError
        app.MapPost("/animals", PostAnimal);
        // Contract: Empty or ApiError
        app.MapDelete("/animals/{id}", DeleteAnimalById);
        // Contract: Empty or ApiError
        app.MapPut("/animals/{id}", PutAnimalById);
    }

    private async static Task<IResult> GetAnimals(Supabase.Client client)
    {
        var repo = new SupaBaseAnimalRepo(client);
        try
        {
            var result = await repo.GetAllAsync();
            return Results.Ok(result);
        }
        catch (Exception e)
        {
            Log.Warning(e, $"Function: {nameof(GetAnimals)}");
            return Results.InternalServerError(e);
        }
    }
    private async static Task<IResult> GetAnimalById(string id, Supabase.Client client)
    {
        var repo = new SupaBaseAnimalRepo(client);
        try
        {
            if (!Guid.TryParse(id, out Guid guid))
            {
                Log.Warning($"Function: {nameof(GetAnimalById)} Failed parse of ID {id}");
                return Results.InternalServerError();
            }
            var result = await repo.GetByIdAsync(guid);
            return Results.Ok(result);
        }
        catch (Exception e)
        {
            Log.Warning(e, $"Function: {nameof(GetAnimals)}");
            return Results.InternalServerError(e);
        }
    }

    private async static Task<IResult> PostAnimal(ClaimsPrincipal user, PostAnimalRequest request, Supabase.Client client)
    {
        if (!JwtHelpers.TryGetUserGuidFromClaim(nameof(PostAnimal), user, out Guid guid, out IResult? error))
        {
            return error;
        }
        try
        {
            var animal = Animal.NewAnimal(request.Name, request.ImageUrl);
            var repo = new SupaBaseAnimalRepo(client);
            repo.Insert(animal);
            return Results.Ok();
        }
        catch (Exception e)
        {
            Log.Warning(e, $"Function: {nameof(PostAnimal)}");
            return Results.InternalServerError(e);
        }
    }

    private async static Task<IResult> DeleteAnimalById(string id, ClaimsPrincipal user, Supabase.Client client)
    {
        if (!JwtHelpers.TryGetUserGuidFromClaim(nameof(PostAnimal), user, out Guid guid, out IResult? error))
        {
            return error;
        }
        try
        {
            if (!Guid.TryParse(id, out Guid identificator))
            {
                return Results.InternalServerError($"Failed id to Guid Parse");
            }
            var repo = new SupaBaseAnimalRepo(client);
            repo.Delete(identificator);
            return Results.Ok();
        }
        catch (Exception e)
        {
            Log.Warning(e, $"Function: {nameof(DeleteAnimalById)}");
            return Results.InternalServerError(e);
        }
    }
    private async static Task<IResult> PutAnimalById(string id, ClaimsPrincipal user, Supabase.Client client, PutAnimalRequest request)
    {
        if (!JwtHelpers.TryGetUserGuidFromClaim(nameof(PostAnimal), user, out Guid guid, out IResult? error))
        {
            return error;
        }
        try
        {
            if (!Guid.TryParse(id, out Guid identificator))
            {
                return Results.InternalServerError($"Failed id to Guid Parse");
            }
            var repo = new SupaBaseAnimalRepo(client);
            var animal = Animal.NewAnimalWithId(identificator, request.Name, request.ImageUrl);
            repo.Update(animal);
            return Results.Ok();
        }
        catch (Exception e)
        {
            Log.Warning(e, $"Function: {nameof(PutAnimalById)}");
            return Results.InternalServerError(e);
        }
    }
}
public record PostAnimalRequest(string Name, string ImageUrl);
public record PutAnimalRequest(string Name, string ImageUrl);