using System.Security.Claims;
using archerly.api.helpers;
using archerly.database.repos;
using archerly.database.repos.interfaces;
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
        app.MapGet("/animals/{id:guid}", GetAnimalById);
        // Contract: Empty or ApiError
        app.MapPost("/animals", PostAnimal);
        // Contract: Empty or ApiError
        app.MapDelete("/animals/{id:guid}", DeleteAnimalById);
        // Contract: Empty or ApiError
        app.MapPut("/animals/{id:guid}", PutAnimalById);
    }

    private async static Task<IResult> GetAnimals(IAnimalRepository repo)
    {
        try
        {
            var result = await repo.GetAllAsync();
            return Results.Ok(result);
        }
        catch (Exception e)
        {
            Log.Warning(e, $"Function: {nameof(GetAnimals)}");
            return Results.InternalServerError();
        }
    }
    private async static Task<IResult> GetAnimalById(Guid id, IAnimalRepository repo)
    {
        try
        {
            var result = await repo.GetByIdAsync(id);
            return Results.Ok(result);
        }
        catch (Exception e)
        {
            Log.Warning(e, $"Function: {nameof(GetAnimals)}");
            return Results.InternalServerError();
        }
    }

    private async static Task<IResult> PostAnimal(ClaimsPrincipal user, PostAnimalRequest request, IAnimalRepository repo)
    {
        if (!JwtHelpers.TryGetUserGuidFromClaim(nameof(PostAnimal), user, out Guid guid, out IResult? error))
        {
            return error;
        }
        try
        {
            var animal = Animal.NewAnimal(request.Name, request.ImageUrl);
            var a = await repo.AddAsync(animal);
            if (a is not null)
            {
                return Results.Ok(a.Id);
            }
            return Results.Ok();
        }
        catch (Exception e)
        {
            Log.Warning(e, $"Function: {nameof(PostAnimal)}");
            return Results.InternalServerError();
        }
    }

    private async static Task<IResult> DeleteAnimalById(Guid id, ClaimsPrincipal user, IAnimalRepository repo)
    {
        if (!JwtHelpers.TryGetUserGuidFromClaim(nameof(PostAnimal), user, out Guid guid, out IResult? error))
        {
            return error;
        }
        try
        {
            await repo.DeleteAsync(id);
            return Results.Ok();
        }
        catch (Exception e)
        {
            Log.Warning(e, $"Function: {nameof(DeleteAnimalById)}");
            return Results.InternalServerError();
        }
    }
    private async static Task<IResult> PutAnimalById(Guid id, ClaimsPrincipal user, PutAnimalRequest request, IAnimalRepository repo)
    {
        if (!JwtHelpers.TryGetUserGuidFromClaim(nameof(PostAnimal), user, out Guid guid, out IResult? error))
        {
            return error;
        }
        try
        {
            var animal = Animal.NewAnimalWithId(id, request.Name, request.ImageUrl);
            await repo.UpdateAsync(animal);
            return Results.Ok();
        }
        catch (Exception e)
        {
            Log.Warning(e, $"Function: {nameof(PutAnimalById)}");
            return Results.InternalServerError();
        }
    }
}
public record PostAnimalRequest(string Name, string ImageUrl);
public record PutAnimalRequest(string Name, string ImageUrl);