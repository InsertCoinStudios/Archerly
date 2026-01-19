using System.Security.Claims;
using archerly.api.helpers;
using archerly.database.repos;
using archerly.database.repos.interfaces;
using archerly.entities;
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
    private async static Task<IResult> GetCourses(Supabase.Client client, ClaimsPrincipal user, IHydratedCourseRepository repo)
    {
        if (!JwtHelpers.TryGetUserGuidFromClaim(nameof(GetCourses), user, out Guid guid, out IResult? error))
        {
            return error;
        }
        try
        {
            var courses = await repo.GetAllAsync();
            return Results.Ok(new AllHydratedCourseResponse(courses));
        }
        catch (Exception e)
        {
            Log.Warning(e, $"Function: {nameof(GetCourses)}");
            return Results.InternalServerError(e);
        }
    }

    // Consumer has to resolve the Animals  himself by suing the provided IDs
    private async static Task<IResult> GetCourseById(string id, Supabase.Client client, ClaimsPrincipal user, IHydratedCourseRepository repo)
    {
        if (!JwtHelpers.TryGetUserGuidFromClaim(nameof(GetCourseById), user, out Guid guid, out IResult? error))
        {
            return error;
        }
        var service = new CourseService(client);
        try
        {
            var courseGuid = Guid.Parse(id);
            var course = await repo.GetByIdAsync(courseGuid) ?? throw new NullReferenceException($"Function: {nameof(GetCourseById)}");
            return Results.Ok(new HydratedCourseResponse(course));
        }
        catch (Exception e)
        {
            Log.Warning(e, $"Function: {nameof(GetCourseById)}");
            return Results.InternalServerError(e);
        }
    }


    private async static Task<IResult> PostCourse(ClaimsPrincipal user, HydratedCourseRequest request, IHydratedCourseRepository repo)
    {
        if (!JwtHelpers.TryGetUserGuidFromClaim(nameof(PostCourse), user, out Guid guid, out IResult? error))
        {
            return error;
        }
        try
        {
            await repo.AddAsync(request.Course);
            return Results.Ok();
        }
        catch (Exception e)
        {
            Log.Warning(e, $"Function: {nameof(PostCourse)}");
            return Results.InternalServerError(e);
        }
    }

    private async static Task<IResult> DeleteCourseById(string id, ClaimsPrincipal user, IHydratedCourseRepository repo)
    {
        if (!JwtHelpers.TryGetUserGuidFromClaim(nameof(DeleteCourseById), user, out Guid guid, out IResult? error))
        {
            return error;
        }
        try
        {
            var courseGuid = Guid.Parse(id);
            await repo.DeleteAsync(courseGuid);
            return Results.Ok();
        }
        catch (Exception e)
        {
            Log.Warning(e, $"Function: {nameof(DeleteCourseById)} ID: {id}");
            return Results.InternalServerError(e);
        }
    }


    private async static Task<IResult> PutCourseById(string id, ClaimsPrincipal user, HydratedCourseRequest request, IHydratedCourseRepository repo)
    {

        if (!JwtHelpers.TryGetUserGuidFromClaim(nameof(PutCourseById), user, out Guid guid, out IResult? error))
        {
            return error;
        }
        try
        {
            var courseGuid = Guid.Parse(id);
            await repo.UpdateAsync(request.Course);
            return Results.Ok();
        }
        catch (Exception e)
        {
            Log.Warning(e, $"Function: {nameof(PutCourseById)} ID: {id}");
            return Results.InternalServerError(e);
        }
    }

}

public record HydratedCourseRequest(HydratedCourse Course);
public record HydratedCourseResponse(HydratedCourse Course);
public record AllHydratedCourseResponse(List<HydratedCourse> Courses);