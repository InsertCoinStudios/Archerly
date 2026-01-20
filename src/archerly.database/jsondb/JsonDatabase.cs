using archerly.entities;
namespace archerly.database.jsondb;

public class JsonDatabase
{
    public List<Animal> Animals { get; set; } = new();
    public List<Course> Courses { get; set; } = new();
    public List<CourseAnimal> CourseAnimals { get; set; } = new();
    public List<User> Users { get; set; } = new();
    public List<Shot> Shots { get; set; } = new();
}