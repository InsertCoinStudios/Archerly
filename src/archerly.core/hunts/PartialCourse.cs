namespace archerly.core;

public class PartialCourse
{
    public string Name { get; set; }
    public string Location { get; set; }
    public string Info { get; set; }
    public Difficulty Difficulty { get; set; }

    public PartialCourse(string Name, string Location, string Info, Difficulty Difficulty)
    {
        this.Name = Name;
        this.Location = Location;
        this.Info = Info;
        this.Difficulty = Difficulty;
    }

    public Course Populate(List<Animal> targets)
    {
        return new Course(this, targets);
    }
}