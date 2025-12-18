namespace archerly.api.endpoints;

public static class AnimalEndpoints
{
    // TODO: Admin only
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
        return Results.Ok();
    }
    private static IResult GetAnimalById(string? id)
    {
        return Results.Ok(id);
    }

    private static IResult PostAnimal()
    {
        return Results.Ok();
    }

    private static IResult DeleteAnimalById(string? id)
    {
        return Results.Ok(id);
    }
    private static IResult PatchAnimalById(string? id)
    {
        return Results.Ok(id);
    }
    private static IResult PutAnimalById(string? id)
    {
        return Results.Ok(id);
    }
}