namespace archerly.core.hunts;

public class HuntSettingsDto
{
    /// <summary>
    /// The scoring variant. Null if not set.
    /// </summary>
    public ShotType? ScoringVariant { get; set; }

    /// <summary>
    /// The selected course. Null if not set.
    /// </summary>
    public entities.HydratedCourse? SelectedCourse { get; set; }

    /// <summary>
    /// Creates a DTO from a finalized HuntSettings object.
    /// </summary>
    public static HuntSettingsDto From(HuntSettings settings) => new()
    {
        ScoringVariant = settings.ScoringVariant,
        SelectedCourse = settings.SelectedCourse
    };

    /// <summary>
    /// Creates a DTO from a PendingHuntSettings object.
    /// </summary>
    public static HuntSettingsDto From(PendingHuntSettings pending) => new()
    {
        ScoringVariant = pending.ScoringVariant,
        SelectedCourse = pending.SelectedCourse
    };

    /// <summary>
    /// Converts the DTO to a finalized HuntSettings instance.
    /// Throws if a required property is missing.
    /// </summary>
    public HuntSettings ToHuntSettings()
    {
        if (!ScoringVariant.HasValue)
            throw new ScoringVariantNotSetException();

        if (SelectedCourse is null)
            throw new CourseNotSetException();

        return new HuntSettings(ScoringVariant.Value, SelectedCourse);
    }

    /// <summary>
    /// Converts the DTO to a PendingHuntSettings instance.
    /// </summary>
    public PendingHuntSettings ToPendingHuntSettings()
    {
        var pending = new PendingHuntSettings();
        pending.ScoringVariant = ScoringVariant;
        pending.SelectedCourse = SelectedCourse;
        return pending;
    }
}