using System.Security.Claims;
using archerly.api.helpers;
using archerly.database.repos;
using Serilog;

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
        app.MapPut("/courses/{id}", PutCourseById);
    }

    // Consumer has to resolve the Animals  himself by suing the provided IDs
    private async static Task<IResult> GetCourses(Supabase.Client client, ClaimsPrincipal user)
    {
        if (!JwtHelpers.TryGetUserGuidFromClaim(nameof(GetCourses), user, out Guid guid, out IResult? error))
        {
            return error;
        }
        var service = new CourseService(client);
        try
        {
            var courses = await service.GetAllAsync();
            return Results.Ok(new AllCourseResponse(courses));
        }
        catch (Exception e)
        {
            Log.Warning(e, $"Function: {nameof(GetCourses)}");
            return Results.InternalServerError(e);
        }
    }

    // Consumer has to resolve the Animals  himself by suing the provided IDs
    private async static Task<IResult> GetCourseById(string id, Supabase.Client client, ClaimsPrincipal user)
    {
        if (!JwtHelpers.TryGetUserGuidFromClaim(nameof(GetCourseById), user, out Guid guid, out IResult? error))
        {
            return error;
        }
        var service = new CourseService(client);
        try
        {
            var courseGuid = Guid.Parse(id);
            var course = await service.GetByIdAsync(courseGuid) ?? throw new NullReferenceException($"Function: {nameof(GetCourseById)}");
            return Results.Ok(new CourseResponse(course));
        }
        catch (Exception e)
        {
            Log.Warning(e, $"Function: {nameof(GetCourseById)}");
            return Results.InternalServerError(e);
        }
    }

    private async static Task<IResult> PostCourse(ClaimsPrincipal user, CourseRequest request, Supabase.Client client)
    {
        if (!JwtHelpers.TryGetUserGuidFromClaim(nameof(PostCourse), user, out Guid guid, out IResult? error))
        {
            return error;
        }
        var service = new CourseService(client);
        try
        {
            await service.InsertCourseAsync(request.Course);
            return Results.Ok();
        }
        catch (Exception e)
        {
            Log.Warning(e, $"Function: {nameof(PostCourse)}");
            return Results.InternalServerError(e);
        }
    }

    private async static Task<IResult> DeleteCourseById(string id, ClaimsPrincipal user, Supabase.Client client)
    {
        if (!JwtHelpers.TryGetUserGuidFromClaim(nameof(DeleteCourseById), user, out Guid guid, out IResult? error))
        {
            return error;
        }
        var service = new CourseService(client);
        try
        {
            var courseGuid = Guid.Parse(id);
            await service.DeleteCourseAsync(courseGuid);
            return Results.Ok();
        }
        catch (Exception e)
        {
            Log.Warning(e, $"Function: {nameof(DeleteCourseById)} ID: {id}");
            return Results.InternalServerError(e);
        }
    }

    private async static Task<IResult> PutCourseById(string id, ClaimsPrincipal user, CourseRequest request, Supabase.Client client)
    {

        if (!JwtHelpers.TryGetUserGuidFromClaim(nameof(PutCourseById), user, out Guid guid, out IResult? error))
        {
            return error;
        }
        var service = new CourseService(client);
        try
        {
            var courseGuid = Guid.Parse(id);
            await service.UpdateCourseAsync(courseGuid, request.Course);
            return Results.Ok();
        }
        catch (Exception e)
        {
            Log.Warning(e, $"Function: {nameof(PutCourseById)} ID: {id}");
            return Results.InternalServerError(e);
        }
    }
}

public record CourseRequest(CourseDto Course);
public record CourseResponse(CourseDto Course);
public record AllCourseResponse(List<CourseDto> Courses);