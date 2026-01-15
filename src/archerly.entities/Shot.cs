using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace archerly.entities;
[Table("turn_shots")]
public class Shot:BaseModel
{
    [PrimaryKey("id")]
    public int Id { get; init; }
    [Column("shot_number")]
    public int ShotNumber { get; init; }
    [Column("kind")]
    public string Kind { get; init; }
    [Column("score")]
    public double Score { get; init; }
    [Column("animal_id")]
    public int AnimalId { get; init; }
    [Column("user_id")]
    public int UserId { get; init; }
    
}