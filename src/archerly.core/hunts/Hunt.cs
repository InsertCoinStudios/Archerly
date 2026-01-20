namespace archerly.core.hunts;

public class Hunt
{
    public ScoreBoard Scores
    {
        get
        {
            lock (_scoreLock)
            {
                return _scores;
            }
        }
        private set
        {
            lock (_scoreLock)
            {
                _scores = value;
            }
        }
    }

    private ScoreBoard _scores;
    private readonly Course _course;
    private readonly ShotType _scoringVariant;
    public string SessionId { get; private set; }
    public HuntSettings Settings { get; private set; }
    public PlayerList Players { get; init; }

    private readonly Lock _scoreLock = new();

    public Hunt(HuntSettings settings, PendingHunt partial)
    {
        SessionId = partial.SessionId;
        Players = partial.Players;
        _scoringVariant = settings.ScoringVariant;
        _course = settings.SelectedCourse;
        _scores = new ScoreBoard(_scoringVariant, _course.Targets);
        Settings = settings;
    }
}