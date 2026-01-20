namespace archerly.core.hunts;

public class HuntSettings
{
    public ShotType ScoringVariant { get; set; }
    public entities.HydratedCourse SelectedCourse { get; set; }

    public HuntSettings(ShotType scoringVariant, entities.HydratedCourse selected)
    {
        ArgumentNullException.ThrowIfNull(selected);
        ScoringVariant = scoringVariant;
        SelectedCourse = selected;
    }
}