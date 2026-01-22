
using System.Security.Claims;
using archerly.api.helpers;
using archerly.core.hunts;
using archerly.database.repos.interfaces;
using archerly.entities;
using Serilog;

namespace archerly.api.endpoints;

public static class HuntsEndPoint
{
    public static void MapHuntEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/hunts/{id:length(4)}", GetHuntById).RequireAuthorization();
        app.MapGet("/hunts/{id:length(4)}/IsActivated", GetHuntIsActivated).RequireAuthorization();
        app.MapGet("/hunts/{id:length(4)}/GetCourse", GetCourseIDByHuntId).RequireAuthorization();
        app.MapGet("/hunts/{id:length(4)}/stats", GetHuntStats).RequireAuthorization();
        app.MapGet("/hunts/{id:length(4)}/userstats", GetHuntUserStats).RequireAuthorization();
        app.MapPost("/hunts", PostHunt).RequireAuthorization();
        app.MapPost("/hunts/{id:length(4)}/join", PostHuntJoinById).RequireAuthorization();
        app.MapPost("/hunts/{id:length(4)}/leave", PostHuntLeaveById).RequireAuthorization();

        app.MapPost("/hunts/{id:length(4)}/shotvariant", PostHuntScoreVariantById).RequireAuthorization();
        app.MapPost("/hunts/{id:length(4)}/course", PostHuntCourseById).RequireAuthorization();
        app.MapPost("/hunts/{id:length(4)}/activate", PostHuntActivateById).RequireAuthorization();
        app.MapPost("/hunts/{huntId:length(4)}/animals/{animalId:guid}/shot/{shotCount}", PostHuntShotOnTargetByIds).RequireAuthorization();
    }

    private static async Task<IResult> GetHuntIsActivated(string id, HuntManager manager, ClaimsPrincipal user)
    {
        if (!JwtHelpers.TryGetUserGuidFromClaim("GetHunt", user, out Guid guid, out IResult? error))
        {
            return error;
        }
        var active = manager.IsActivated(id);
        return Results.Ok(active);
    }


    // returns the data for the requested
    // Does not show the data from the settings
    private async static Task<IResult> GetHuntById(string id, ClaimsPrincipal user, HuntManager manager, IUserRepository repo)
    {
        if (!JwtHelpers.TryGetUserGuidFromClaim("GetHunt", user, out Guid guid, out IResult? error))
        {
            return error;
        }

        var data = manager.GetDataFor(id, guid);
        if (data is null)
        {
            Log.Information($"No Valid Session found for {id}");
            return Results.NotFound();
        }
        // resolve players and owner
        var resolvedPlayers = new List<User>();
        foreach (var innerUser in data.Players)
        {
            var resolvedUser = await repo.GetByIdAsync(innerUser);
            if (resolvedUser is null)
            {
                Log.Warning($"Illegal State could not resolve Logged in User {innerUser} Function: GetHuntById");
                // Remove this invalid state player
                //manager.PlayerLeft(id, innerUser);// removed because i think it conflicts
                continue;
            }
            resolvedPlayers.Add(resolvedUser);
        }
        var resolvedOwner = await repo.GetByIdAsync(data.Owner)
        ?? throw new InvalidDataException("Owner has to be Resolveable");
        var response = new HuntDataResponse(resolvedOwner, resolvedPlayers, resolvedPlayers.Count, data.SessionId);
        return Results.Ok(response);
    }


    /// <summary>
    /// Create a new Hunt from Scratch
    /// Returns a Session Id to reference the Hunt in the future
    /// </summary>
    /// <returns></returns>
    private async static Task<IResult> PostHunt(ClaimsPrincipal user, Supabase.Client client, HuntManager manager, IUserRepository repo)
    {
        if (!JwtHelpers.TryGetUserGuidFromClaim(nameof(PostHunt), user, out Guid guid, out IResult? error))
        {
            return error;
        }
        try
        {
            var huntId = manager.CreateNewPendingHunt(guid);
            var data = await GetHuntById(huntId, user, manager, repo);
            return Results.Ok(data);
        }
        catch (Exception e)
        {
            Log.Warning(e, $"Function: PostHunt");
            return Results.InternalServerError();
        }
    }

    private async static Task<IResult> PostHuntJoinById(string id, ClaimsPrincipal user, HuntManager manager)
    {
        if (!JwtHelpers.TryGetUserGuidFromClaim(nameof(PostHuntJoinById), user, out Guid guid, out IResult? error))
        {
            return error;
        }
        var res = manager.PlayerJoined(id, guid);
        if (res)
        {
            return Results.Ok();
        }
        return Results.NotFound();
    }
    private async static Task<IResult> PostHuntLeaveById(string id, ClaimsPrincipal user, HuntManager manager)
    {
        if (!JwtHelpers.TryGetUserGuidFromClaim(nameof(PostHuntLeaveById), user, out Guid guid, out IResult? error))
        {
            return error;
        }
        var res = manager.PlayerLeft(id, guid);
        if (res)
        {
            return Results.Ok();
        }
        return Results.NotFound();
    }

    private async static Task<IResult> PostHuntScoreVariantById(string id, ClaimsPrincipal user, HuntManager manager, HuntScoreVariantSetRequest receivedData)
    {
        if (!JwtHelpers.TryGetUserGuidFromClaim(nameof(PostHuntScoreVariantById), user, out Guid guid, out IResult? error))
        {
            return error;
        }
        try
        {
            manager.SetScoringVariantForPendingHunt(id, receivedData.Variant);
            return Results.Ok();
        }
        catch (Exception e)
        {
            Log.Warning(e, "PostHuntScoreVariantById");
            return Results.InternalServerError();
        }
    }


    private async static Task<IResult> PostHuntCourseById(string id, ClaimsPrincipal user, HuntManager manager, HuntCourseSetRequest receivedData, IHydratedCourseRepository repo)
    {
        if (!JwtHelpers.TryGetUserGuidFromClaim(nameof(PostHuntCourseById), user, out Guid guid, out IResult? error))
        {
            return error;
        }
        try
        {
            var hydrated = await repo.GetByIdAsync(receivedData.CourseId);
            if (hydrated is null)
            {
                return Results.NotFound();
            }
            manager.SetCourseForPendingHunt(id, hydrated);
            return Results.Ok();
        }
        catch (Exception e)
        {
            Log.Warning(e, "PostHuntScoreVariantById");
            return Results.InternalServerError();
        }
    }


    private async static Task<IResult> PostHuntActivateById(string id, ClaimsPrincipal user, HuntManager manager)
    {
        if (!JwtHelpers.TryGetUserGuidFromClaim(nameof(PostHuntActivateById), user, out Guid guid, out IResult? error))
        {
            return error;
        }
        try
        {
            manager.ActivatePendingHunt(id);
        }
        catch (Exception e)
        {
            return Results.InternalServerError();
        }
        return Results.Ok();
    }

    private async static Task<IResult> PostHuntShotOnTargetByIds(string huntId, Guid animalId, string shotCount, ClaimsPrincipal user, HuntManager manager, HuntRegisterShotRequest receivedData, IShotRepository repo)
    {
        if (!JwtHelpers.TryGetUserGuidFromClaim(nameof(PostHuntShotOnTargetByIds), user, out Guid guid, out IResult? error))
        {
            return error;
        }
        try
        {
            var shotNumber = int.Parse(shotCount);
            var shotEntity = manager.SaveShot(huntId, guid, animalId, receivedData.PointsScored, shotNumber);
            await repo.AddAsync(shotEntity);
            return Results.Ok();
        }
        catch (Exception e)
        {
            Log.Warning(e, $"Function: PostHuntShotOnTarget ID: {huntId} AnimalId: {animalId}");
            return Results.InternalServerError();
        }
    }

    private async static Task<IResult> GetHuntStats(string id, ClaimsPrincipal user, HuntManager manager, IUserRepository repo)
    {
        if (!JwtHelpers.TryGetUserGuidFromClaim(nameof(GetHuntStats), user, out Guid guid, out IResult? error))
        {
            return error;
        }
        try
        {
            var stats = manager.GetStatsFor(id);
            return Results.Ok(await StatConverter.From(stats, repo));
        }
        catch (Exception e)
        {
            Log.Warning(e, $"Function: {nameof(GetHuntStats)} ID: {id}");
            return Results.InternalServerError();
        }

    }

    private async static Task<IResult> GetHuntUserStats(string id, ClaimsPrincipal user, HuntManager manager, IUserRepository repo)
    {
        if (!JwtHelpers.TryGetUserGuidFromClaim(nameof(GetHuntUserStats), user, out Guid guid, out IResult? error))
        {
            return error;
        }
        try
        {
            Log.Information("Player getting his stats is with guid {guid}", guid);
            var stats = manager.GetUserStatsFor(id, guid);
            Log.Information("stats: {@stats}", stats);
            return Results.Ok(await StatConverter.From(stats, repo));
        }
        catch (Exception e)
        {
            Log.Warning(e, $"Function: {nameof(GetHuntUserStats)} ID: {id}");
            return Results.InternalServerError();
        }

    }
    private async static Task<IResult> GetCourseIDByHuntId(string id, ClaimsPrincipal user, HuntManager manager)
    {

        if (!JwtHelpers.TryGetUserGuidFromClaim(nameof(GetHuntUserStats), user, out Guid guid, out IResult? error))
        {
            return error;
        }
        try
        {
            Log.Information("Getting course id via hunt id");
            var course = manager.GetCourseViaHunt(id);
            return Results.Ok(course);

        }
        catch (Exception e)
        {
            Log.Warning(e, $"Function: {nameof(GetCourseIDByHuntId)} ID: {id}");
            return Results.InternalServerError();
        }
    }
}

public static class StatConverter
{
    public static async Task<HuntUserStatResponse> From(UserStats stats, IUserRepository repo)
    {
        var user = await repo.GetByIdAsync(stats.User);
        HuntUserStatsWrapper temp;
        if (user is null)
        {
            temp = new HuntUserStatsWrapper(stats.User, "Babbabooie", stats.Kill, stats.Hit, stats.Miss, stats.Rank);
        }
        else
        {
            var name = user.Nickname;
            temp = new HuntUserStatsWrapper(stats.User, name, stats.Kill, stats.Hit, stats.Miss, stats.Rank);
        }
        return new HuntUserStatResponse(temp);
    }

    //public record AllStats(List<ScoreBoardData> Ranking);
    public static async Task<ListedRanks> From(AllStats stats, IUserRepository repo)
    {
        List<AllStatWrapper> listing = new();
        var oldRanking = stats.Ranking;
        foreach (var rank in oldRanking)
        {
            listing.Add(await From(rank, repo));
        }
        return new ListedRanks(listing);
    }

    public static async Task<AllStatWrapper> From(ScoreBoardData singularRank, IUserRepository repo)
    {
        var user = await repo.GetByIdAsync(singularRank.UserId);
        AllStatWrapper result;
        if (user is null)
        {
            result = new AllStatWrapper(singularRank.UserId, "Babbabooie", singularRank.Rank, singularRank.Score);
        }
        else
        {
            var name = user.Nickname;
            result = new AllStatWrapper(singularRank.UserId, name, singularRank.Rank, singularRank.Score);
        }
        return result;
    }
}
public record HuntUserStatsWrapper(Guid User, string UserName, int Kill, int Hit, int Miss, int Rank);
public record AllStatWrapper(Guid User, string UserName, int Rank, int Score);
public record ListedRanks(List<AllStatWrapper> Ranks);
//public record AllStats(List<KeyValuePair<Guid, int>> Ranking);

public record HuntDataResponse(User Owner, List<User> Players, long PlayerCount, string SessionId);
public record HuntScoreVariantSetRequest(int Variant);
public record HuntCourseSetRequest(Guid CourseId);
public record HuntRegisterShotRequest(int PointsScored);
public record HuntAllStatResponse(ListedRanks Stats);
public record HuntUserStatResponse(HuntUserStatsWrapper Stats);