using System.Security.Claims;
using archerly.api.helpers;

namespace archerly.api.endpoints;

public static class ImageEndpoint
{

    public static void MapImageEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/images", GetImages);
        app.MapGet("/images/{id}", GetImage);
        app.MapPost("/images", PostImage);
    }

    private static IResult GetImages()
    {
        // TODO: Get Images
        // Retrieve all Images
        // Note this endpoint can be expensive maybe opt for an Metadata providing endpoint
        // That way only required Images con be fetched
        return Results.Ok();
    }

    /// <summary>
    /// Retrieves an Image from the Database and provides it as a downloadable resource
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    private static IResult GetImage(string id)
    {
        // TODO: Get Image
        // Query Database for the Image data
        return Results.Ok();
    }

    /// <summary>
    /// Receives image Data and converts it for persitance in the Database
    /// </summary>
    /// <returns></returns>
    private static IResult PostImage()
    {
        // TODO: Post Image
        // Convert uploaded Image data into a form that can be persisted in the db
        return Results.Ok();
    }
}