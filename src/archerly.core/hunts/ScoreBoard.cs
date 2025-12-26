
using archerly.core.extensions;

namespace archerly.core.hunts;

public class ScoreBoard
{
    private readonly List<Shot> _shots = new();
    private readonly Dictionary<User, long> _playerPoints = new();
    private readonly ShotType _shotType = ShotType.ThreeShot;
    private readonly Lock _lock = new();

    public ScoreBoard(ShotType selectedVariant)
    {
        _shotType = selectedVariant;
    }

    public void RegisterShot(User Player, Animal Target, long Points)
    {
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

}