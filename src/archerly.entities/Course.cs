using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace archerly.entities;

[Table("courses")]
public class Course : BaseModel
{
    [PrimaryKey("id")]
    public Guid Id { get; set; }
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
    [Column("name")]
    public string Name { get; set; }
    [Column("location")]
    public string Location { get; set; }
    [Column("difficulty_id")]
    public int DifficultyId { get; set; }
    public string Difficulty { get; set; }
    public Array Animals { get; set; }
    [Column("info")]
    public string Info { get; set; }

    public static Course From(string name, string location, string info, int difficulty, Array targets)
    {
        var result = new Course();
        result.Name = name;
        result.Location = location;
        result.Info = info;
        result.DifficultyId = difficulty;
        result.Difficulty = difficulty switch
        {
            0 => "easy",
            1 => "medium",
            2 => "hard",
            _ => "" // default case for anything else
        };
        result.Animals = targets;
        return result;
    }
}