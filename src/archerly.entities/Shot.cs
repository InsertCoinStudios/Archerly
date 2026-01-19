using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace archerly.entities;
[Table("turn_shots")]
public class Shot:BaseModel
{
    [PrimaryKey("id")]
    public Guid Id { get; init; }
    [Column("huntid")]
    public Guid HuntId { get; init; }
    [Column("shotnumber")]
    public int ShotNumber { get; set; }
    [Column("kind")]
    public int Kind { get; set; }
    [Column("score")]
    public int Score { get; set; }
    [Column("animalid")]
    public Guid AnimalId { get; set; }
    [Column("userid")]
    public Guid UserId { get; set; }
    
}