using System.Diagnostics.Metrics;
using System.Security.Claims;
using System.Threading.Tasks;
using archerly.api.helpers;
using archerly.database.repos;
using archerly.database.repos.interfaces;
using Serilog;

namespace archerly.api.endpoints;

public static class AllTimeStatsEndpoint
{

    public static void MapAllTimeStatEndpoints(this IEndpointRouteBuilder app)
    {
        // Contract DTO of AllTimeStats or APIError DTO
        app.MapPost("/allTimeStats", GetAllTimeStats);
    }

    private static async Task<IResult> GetAllTimeStats(ClaimsPrincipal user, IShotRepository repo)
    {
        if (!JwtHelpers.TryGetUserGuidFromClaim(nameof(GetAllTimeStats), user, out Guid guid, out IResult? error))
        {
            return error;
        }
        List<entities.Shot> shots;
        try
        {
            shots = await repo.GetAllByPlayerAsync(guid);
        }
        catch (Exception e)
        {
            Log.Warning(e, $"Function: {nameof(GetAllTimeStats)} GUID: {guid}");
            throw;
        }
        var counterKillShot = 0;
        var counterHit = 0;
        var counterMiss = 0;
        foreach (var shot in shots)
        {
            if (shot.Score == 0)
            {
                counterMiss++;
            }
            if (shot.Score > 0)
            {
                counterHit++;
            }
            if (IsKillShot(shot))
            {
                counterKillShot++;
            }
        }
        var response = new AllTimeStatResponse(counterKillShot, counterHit, counterMiss, shots);

        return Results.Ok(response);
    }

    private static bool IsKillShot(entities.Shot shot)
    {
        if (shot.Score == 0)
        {
            return false;
        }

        // Zweipfeil 20 Score immer Kill
        // Dreipfeil Erster Pfeil 20 Kill
        // Dreipfeil Zweiter 16 Kill
        // Dreipfeil Dritter 10 Kill
        return shot switch
        {
            // Zweipfeil: always kill on 20
            { Kind: 2, Score: 20 } => true,

            // Dreipfeil rules
            { Kind: 3, ShotNumber: 1, Score: 20 } => true,
            { Kind: 3, ShotNumber: 2, Score: 16 } => true,
            { Kind: 3, ShotNumber: 3, Score: 10 } => true,

            // everything else
            _ => false
        };
    }
}

public record AllTimeStatResponse(int Kill, int Hit, int Miss, List<entities.Shot> Shots);