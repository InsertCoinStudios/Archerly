using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace archerly.entities;

[Table("courses")]
public class Course:BaseModel
{
    [PrimaryKey("id")]
    public Guid Id { get; init; }
    [Column("created_at")]
    public DateTime CreatedAt { get; init; }
    [Column("name")]
    public string Name { get; init; }
    [Column("location")]
    public string Location { get; init; }
    [Column("difficulty_id")]
    public int DifficultyId { get; init; }
    public string Difficulty { get; set; }
    public Array Animals { get; set; }
    [Column("info")]
    public string Info { get; init; }
}