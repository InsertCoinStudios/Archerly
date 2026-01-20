using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace archerly.entities;

//[Table("shot")]
public class Shot
//: BaseModel
{
    //    [PrimaryKey("id")]
    public Guid Id { get; set; }

    //    [Column("userid")]
    public Guid UserId { get; set; }

    //    [Column("animalid")]
    public Guid AnimalId { get; set; }

    //    [Column("score")]
    public long Score { get; set; }

    //    [Column("kind")]
    public int Kind { get; set; }

    //    [Column("shotnumber")]
    public long ShotNumber { get; set; }

    //    [Column("huntid")]
    public Guid HuntId { get; set; }

    public static Shot From(Guid id, Guid user_id, Guid animal_id, long score, int kind, long shotNumber, Guid huntId)
    {
        var shot = new Shot
        {
            Id = id,
            UserId = user_id,
            AnimalId = animal_id,
            Score = score,
            Kind = kind,
            ShotNumber = shotNumber,
            HuntId = huntId,
        };
        return shot;
    }
}