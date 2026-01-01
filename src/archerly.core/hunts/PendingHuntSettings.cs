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

    /// <summary>
    /// Builds a <see cref="HuntSettings"/> instance from the current pending hunt settings.
    /// </summary>
    /// <remarks>
    /// This method ensures that both the scoring variant and selected course are set before creating the <see cref="HuntSettings"/>.  
    /// Thread safety is ensured during the build process.
    /// </remarks>
    /// <returns>
    /// A fully populated <see cref="HuntSettings"/> instance containing the scoring variant and selected course.
    /// </returns>
    /// <exception cref="ScoringVariantNotSetException">
    /// Thrown if the scoring variant has not been set in the pending hunt settings.
    /// </exception>
    /// <exception cref="CourseNotSetException">
    /// Thrown if the selected course has not been set in the pending hunt settings.
    /// </exception>
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
