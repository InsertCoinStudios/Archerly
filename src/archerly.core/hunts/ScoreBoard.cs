
using archerly.core.extensions;
using Microsoft.VisualBasic;

namespace archerly.core.hunts;

public class ScoreBoard
{
    private readonly List<Shot> _shots = new();
    // User Guids
    private readonly Dictionary<Guid, int> _playerPoints = new();
    private readonly ShotType _shotType;
    // Guids of the Animals
    private readonly List<Guid> _targets;
    private readonly Lock _lock = new();

    public ScoreBoard(ShotType selectedVariant, List<Guid> targets)
    {
        _shotType = selectedVariant;
        _targets = targets;
    }

    public entities.Shot RegisterShot(Guid Player, Guid Target, int Points, int shotNumber)
    {
        if (!_targets.Contains(Target))
        {
            throw new InvalidTargetForTargetListException(Target, _targets);
        }
        var shot = new Shot(Player, Target, _shotType, Points, shotNumber);
        lock (_lock)
        {
            _shots.Add(shot);
            _playerPoints.AddToCount(Player, Points);
            var conv = shot.Convert();
            return conv;
        }
    }

    public List<KeyValuePair<Guid, int>> GetRanking()
    {
        List<KeyValuePair<Guid, int>> result;
        lock (_lock)
        {
            result = _playerPoints
                .OrderByDescending(kv => kv.Value)
                .ToList();
        }
        return result;
    }

    public Dictionary<Guid, long> GetPointsByTarget(Guid target)
    {
        var result = new Dictionary<Guid, long>();

        lock (_lock)
        {
            foreach (var player in _playerPoints.Keys)
            {
                result[player] = 0;
            }

            foreach (var shot in _shots)
            {
                if (shot.Target.Equals(target))
                {
                    result[shot.Player] += shot.Points;
                }
            }
        }

        return result;
    }

    public List<Shot> GetShotsForPlayer(Guid player)
    {
        List<Shot> result = new();
        foreach (var shot in _shots)
        {
            if (player.Equals(shot.Player))
            {
                result.Add(shot);
            }
        }
        return result;
    }

    public Dictionary<Guid, List<Shot>> GetShotsGroupedByPlayers()
    {
        var result = new Dictionary<Guid, List<Shot>>();
        foreach (var shot in _shots)
        {
            var player = shot.Player;
            if (!result.ContainsKey(player))
            {
                result[player] = new();
            }
            else
            {
                result[player].Add(shot);
            }
        }
        return result;
    }
}

public class InvalidTargetForTargetListException : Exception, IApiErrorConvertible, IDetailProvider
{
    public IDictionary<string, object?> Details { get; init; } = new Dictionary<string, object?>();
    public InvalidTargetForTargetListException(Guid target, List<Guid> targetList)
    : base($"The Target {target.ToString()}, is not valid for Target List [ {Strings.Join(targetList.Select(a => a.ToString()).ToArray(), ", ")}]")
    {
        Details.Add("target_guid", target);
        Details.Add("target_list", targetList);
    }

    public ApiError ToApiError()
    {
        var result = new ApiError("invalid_target_for_targets", "The given Target is not a valid Target for the provided target List");
        result.MergeDetails(this);
        throw new NotImplementedException();
    }
}