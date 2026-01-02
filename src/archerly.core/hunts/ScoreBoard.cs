
using archerly.core.extensions;
using Microsoft.VisualBasic;

namespace archerly.core.hunts;

public class ScoreBoard
{
    private readonly List<Shot> _shots = new();
    // User Guids
    private readonly Dictionary<Guid, long> _playerPoints = new();
    private readonly ShotType _shotType;
    // Guids of the Animals
    private readonly List<Guid> _targets;
    private readonly Lock _lock = new();

    public ScoreBoard(ShotType selectedVariant, List<Guid> targets)
    {
        _shotType = selectedVariant;
        _targets = targets;
    }

    public void RegisterShot(Guid Player, Guid Target, long Points)
    {
        if (!_targets.Contains(Target))
        {
            throw new InvalidTargetForTargetListException(Target, _targets);
        }
        var shot = new Shot(Player, Target, _shotType, Points);
        lock (_lock)
        {
            _shots.Add(shot);
            _playerPoints.AddToCount(Player, Points);
            // TODO: Persist Data into DB
        }
    }

    public List<KeyValuePair<Guid, long>> GetRanking()
    {
        List<KeyValuePair<Guid, long>> result;
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
}

public class InvalidTargetForTargetListException : Exception
{
    public InvalidTargetForTargetListException(Guid target, List<Guid> targetList)
    : base($"The Target {target.ToString()}, is not valid for Target List [ {Strings.Join(targetList.Select(a => a.ToString()).ToArray(), ", ")}]") { }
}