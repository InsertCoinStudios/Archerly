namespace archerly.core.hunts;


public class PendingHuntSettings
{
    private ShotType? _scoringVariant;
    private Course? _selectedCourse;

    private readonly Lock _accessLock = new();

    public ShotType? ScoringVariant
    {
        get
        {
            lock (_accessLock)
            {
                return _scoringVariant;
            }
        }
        set
        {
            lock (_accessLock)
            {
                _scoringVariant = value;
            }
        }
    }

    public Course? SelectedCourse
    {
        get
        {
            lock (_accessLock)
            {
                return _selectedCourse;
            }
        }
        set
        {
            lock (_accessLock)
            {
                _selectedCourse = value;
            }
        }
    }

    public HuntSettings Build()
    {
        lock (_accessLock)
        {
            if (!_scoringVariant.HasValue)
            {
                throw new ScoringVariantNotSetException();
            }

            if (_selectedCourse is null)
            {
                throw new CourseNotSetException();
            }

            return new HuntSettings(
                _scoringVariant.Value,
                _selectedCourse
            );
        }
    }
}


public class ScoringVariantNotSetException : Exception
{
    public ScoringVariantNotSetException()
        : base("HuntSettings has unset ScoringVariant.")
    {
    }
}

public class CourseNotSetException : Exception
{
    public CourseNotSetException()
        : base("HuntSettings has unset SelectedCourse.")
    {
    }
}
