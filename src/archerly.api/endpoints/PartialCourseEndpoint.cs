using System.Security.Claims;
using archerly.api.helpers;
using archerly.database.repos;
using archerly.database.repos.interfaces;
using archerly.entities;
using Serilog;

namespace archerly.api.endpoints;

public static class PartialCoursesEndpoint
{

    public static void MapPartialCourseEndpoints(this IEndpointRouteBuilder app)
    {
        // Contract: List<Courses> or ApiError
        app.MapGet("/courses/partial", GetCoursesPartial);
        // Contract: Course or ApiError
        app.MapGet("/courses/{id}/partial", GetCourseByIdPartial);
        // Contract: Empty or ApiError
        app.MapPost("/courses/partial", PostCoursePartial);
        // Contract: Empty or ApiError
        app.MapDelete("/courses/{id}?partial", DeleteCourseByIdPartial);
        // Contract: Empty or ApiError
        app.MapPut("/courses/{id}/partial", PutCourseByIdPartial);
    }


    private async static Task<IResult> GetCoursesPartial(Supabase.Client client, ClaimsPrincipal user, ICourseRepository repo)
    {
        if (!JwtHelpers.TryGetUserGuidFromClaim(nameof(GetCoursesPartial), user, out Guid guid, out IResult? error))
        {
            return error;
        }
        try
        {
            var courses = await repo.GetAllAsync();
            return Results.Ok(new AllPartialCourseResponse(courses));
        }
        catch (Exception e)
        {
            Log.Warning(e, $"Function: {nameof(GetCoursesPartial)}");
            return Results.InternalServerError(e);
        }
    }

    private async static Task<IResult> GetCourseByIdPartial(string id, Supabase.Client client, ClaimsPrincipal user, ICourseRepository repo)
    {
        if (!JwtHelpers.TryGetUserGuidFromClaim(nameof(GetCourseByIdPartial), user, out Guid guid, out IResult? error))
        {
            return error;
        }
        var service = new CourseService(client);
        try
        {
            var courseGuid = Guid.Parse(id);
            var course = await repo.GetByIdAsync(courseGuid) ?? throw new NullReferenceException($"Function: {nameof(GetCourseByIdPartial)}");
            return Results.Ok(new PartialCourseResponse(course));
        }
        catch (Exception e)
        {
            Log.Warning(e, $"Function: {nameof(GetCourseByIdPartial)}");
            return Results.InternalServerError(e);
        }
    }

    private async static Task<IResult> PostCoursePartial(ClaimsPrincipal user, PartialCourseRequest request, ICourseRepository repo)
    {
        if (!JwtHelpers.TryGetUserGuidFromClaim(nameof(PostCoursePartial), user, out Guid guid, out IResult? error))
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
            Log.Warning(e, $"Function: {nameof(PostCoursePartial)}");
            return Results.InternalServerError(e);
        }
    }

    private async static Task<IResult> DeleteCourseByIdPartial(string id, ClaimsPrincipal user, ICourseRepository repo)
    {
        if (!JwtHelpers.TryGetUserGuidFromClaim(nameof(DeleteCourseByIdPartial), user, out Guid guid, out IResult? error))
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
            Log.Warning(e, $"Function: {nameof(DeleteCourseByIdPartial)} ID: {id}");
            return Results.InternalServerError(e);
        }
    }

    private async static Task<IResult> PutCourseByIdPartial(string id, ClaimsPrincipal user, PartialCourseRequest request, ICourseRepository repo)
    {

        if (!JwtHelpers.TryGetUserGuidFromClaim(nameof(PutCourseByIdPartial), user, out Guid guid, out IResult? error))
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
            Log.Warning(e, $"Function: {nameof(PutCourseByIdPartial)} ID: {id}");
            return Results.InternalServerError(e);
        }
    }
}

public record PartialCourseRequest(Course Course);
public record PartialCourseResponse(Course Course);
public record AllPartialCourseResponse(List<Course> Courses);