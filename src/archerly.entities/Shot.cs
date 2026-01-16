using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace archerly.entities;

[Table("turn_shots")]
public class Shot : BaseModel
{
    [PrimaryKey("id")]
    public int Id { get; init; }
    [Column("shot_number")]
    public int ShotNumber { get; set; }
    [Column("kind")]
    public string Kind { get; set; }
    [Column("score")]
    public int Score { get; set; }
    [Column("animal_id")]
    public Guid AnimalId { get; set; }
    [Column("user_id")]
    public Guid UserId { get; set; }

}