namespace archerly.api.endpoints;

public static class CoursesEndpoint
{

    public static void MapCourseEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/courses", GetCourses);
        app.MapGet("/courses/{id}", GetCourseById);
        app.MapPost("/courses", PostCourse);
        app.MapDelete("/courses/{id}", DeleteCourseById);
        app.MapPatch("/courses/{id}", PatchCourseById);
        app.MapPut("/courses/{id}", PutCourseById);
    }

    private static IResult GetCourses()
    {
        return Results.Ok();
    }

    private static IResult GetCourseById(string? id)
    {
        return Results.Ok(id);
    }

    private static IResult PostCourse()
    {
        return Results.Ok();
    }

    private static IResult DeleteCourseById(string? id)
    {
        return Results.Ok(id);
    }

    private static IResult PatchCourseById(string? id)
    {
        return Results.Ok(id);
    }

    private static IResult PutCourseById(string? id)
    {
        return Results.Ok(id);
    }
}