using System.Security.Claims;

namespace archerly.api.endpoints;

public static class CoursesEndpoint
{

    public static void MapCourseEndpoints(this IEndpointRouteBuilder app)
    {
        // Contract: List<Courses> or ApiError
        app.MapGet("/courses", GetCourses);
        // Contract: Course or ApiError
        app.MapGet("/courses/{id}", GetCourseById);
        // Contract: Empty or ApiError
        app.MapPost("/courses", PostCourse);
        // Contract: Empty or ApiError
        app.MapDelete("/courses/{id}", DeleteCourseById);
        // Contract: Empty or ApiError
        app.MapPatch("/courses/{id}", PatchCourseById);
        // Contract: Empty or ApiError
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