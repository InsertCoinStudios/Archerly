using Supabase.Postgrest.Models;
namespace archerly.entities;

public class HydratedCourse
//: BaseModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Difficulty { get; set; } = string.Empty;
    public string Info { get; set; } = string.Empty;

    public List<Animal> Animals { get; set; } = new List<Animal>();
}