using System.Security.Claims;

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
        // TODO: Get Courses
        // Retrieve all Courses from the DB
        return Results.Ok();
    }

    private static IResult GetCourseById(string? id)
    {
        // TODO: Get Course
        // Retrieve data for the Specific Course from the DB
        return Results.Ok(id);
    }

    private static IResult PostCourse(ClaimsPrincipal user)
    {
        // TODO: Post Course
        // Create a New Course if the user is Admin
        return Results.Ok();
    }

    private static IResult DeleteCourseById(string? id, ClaimsPrincipal user)
    {
        // TODO: Delete Course
        // Delete Course if calling User is Admin
        return Results.Ok(id);
    }

    private static IResult PatchCourseById(string? id, ClaimsPrincipal user)
    {
        // TODO: Patch Course
        // Partially Update Course if User is Admin
        return Results.Ok(id);
    }

    private static IResult PutCourseById(string? id, ClaimsPrincipal user)
    {
        // TODO: Put Course
        // Update Course if user is admin
        return Results.Ok(id);
    }
}