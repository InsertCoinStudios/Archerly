using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace archerly.entities;

[Table("courses")]
public class Course:BaseModel
{
    [PrimaryKey("id")]
    public Guid Id { get; init; }
    [Column("name")]
    public string Name { get; set; }
    [Column("location")]
    public string Location { get; set; }
    [Column("difficulty")]
    public string Difficulty { get; set; }
    public Array Animals { get; set; }
    [Column("info")]
    public string Info { get; init; }
}