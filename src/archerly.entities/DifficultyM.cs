using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace archerly.entities;

[Table("difficulties")]
public class DifficultyM : BaseModel
{
    [PrimaryKey("id", false)]
    public int Id { get; set; }

    [Column("difficulty")]
    public string? DifficultyName { get; set; }
}