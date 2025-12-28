
using archerly.core.extensions;
using Microsoft.VisualBasic;

namespace archerly.core.hunts;

public class ScoreBoard : ICloneable<ScoreBoard>
{
    private readonly List<Shot> _shots = new();
    private readonly Dictionary<User, long> _playerPoints = new();
    private readonly ShotType _shotType;
    private readonly List<Animal> _targets;
    private readonly Lock _lock = new();

    public ScoreBoard(ShotType selectedVariant, List<Animal> targets)
    {
        _shotType = selectedVariant;
        _targets = targets;
    }

    public void RegisterShot(User Player, Animal Target, long Points)
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

    public List<KeyValuePair<User, long>> GetRanking()
    {
        List<KeyValuePair<User, long>> result;
        lock (_lock)
        {
            result = _playerPoints
                .OrderByDescending(kv => kv.Value)
                .ToList();
        }
        return result;
    }

    public Dictionary<User, long> GetPointsByTarget(Animal target)
    {
        var result = new Dictionary<User, long>();

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

    public ScoreBoard Clone()
    {
        lock (_lock)
        {
            // Create a new ScoreBoard with the same ShotType
            var copy = new ScoreBoard(_shotType, _targets);

            // Deep copy shots
            copy._shots.AddRange(_shots);

            // Deep copy player points
            foreach (var kv in _playerPoints)
            {
                copy._playerPoints[kv.Key] = kv.Value;
            }

            return copy;
        }
    }
}

public class InvalidTargetForTargetListException : Exception
{
    public InvalidTargetForTargetListException(Animal target, List<Animal> targetList)
    : base($"The Target {target.Id}, is not valid for Target List [ {Strings.Join(targetList.Select(a => a.Id.ToString()).ToArray(), ", ")}]") { }
}