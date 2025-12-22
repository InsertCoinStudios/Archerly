using System.Security.Claims;

namespace archerly.api.endpoints;

public static class AnimalEndpoints
{
    public static void MapAnimalEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/animal", GetAnimals);
        app.MapGet("/animal/{id}", GetAnimalById);
        app.MapPost("/animal", PostAnimal);
        app.MapDelete("/animal/{id}", DeleteAnimalById);
        app.MapPatch("/animal/{id}", PatchAnimalById);
        app.MapPut("/animal/{id}", PutAnimalById);
    }

    private static IResult GetAnimals()
    {
        // TODO: Get All Animals
        // Retrieve data for all Animals from the db
        return Results.Ok();
    }
    private static IResult GetAnimalById(string? id)
    {
        // TODO: Get specified Animal
        // Retrieve data for the specific Animal
        return Results.Ok(id);
    }

    private static IResult PostAnimal(ClaimsPrincipal user)
    {
        // TODO: Post Animal
        // User has to be Admin
        return Results.Ok();
    }

    private static IResult DeleteAnimalById(string? id, ClaimsPrincipal user)
    {
        // TODO: Delete Animal
        // Delete the specified Animal
        return Results.Ok(id);
    }
    private static IResult PatchAnimalById(string? id, ClaimsPrincipal user)
    {
        // TODO: Patch Animal
        // User has to be admin
        return Results.Ok(id);
    }
    private static IResult PutAnimalById(string? id, ClaimsPrincipal user)
    {
        // TODO: Put Animal
        // User has to be admin
        return Results.Ok(id);
    }
}