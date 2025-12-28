namespace archerly.core.hunts;

public class HuntSettings
{
    public ShotType ScoringVariant { get; set; }
    public Course SelectedCourse { get; set; }

    public HuntSettings(ShotType scoringVariant, Course selected)
    {
        ArgumentNullException.ThrowIfNull(selected);
        ScoringVariant = scoringVariant;
        SelectedCourse = selected;
    }
}