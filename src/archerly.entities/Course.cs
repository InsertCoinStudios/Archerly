using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace archerly.entities;

[Table("courses")]
public class Course : BaseModel
{
    [PrimaryKey("id")]
    public Guid Id { get; set; }

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("location")]
    public string Location { get; set; } = string.Empty;

    [Column("difficulty")]
    public string Difficulty { get; set; } = string.Empty;

    [Column("info")]
    public string Info { get; set; } = string.Empty;

    public static Course From(string name, string location, string info, string difficulty)
    {
        // Normalize input to lowercase for safety
        string diff = difficulty.Trim().ToLower();

        // Validate allowed values
        if (diff != "easy" && diff != "medium" && diff != "hard")
        {
            throw new ArgumentException($"Invalid difficulty '{difficulty}'. Allowed values: easy, medium, hard.");
        }

        return new Course
        {
            Name = name,
            Location = location,
            Info = info,
            Difficulty = diff
        };
    }
}