using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace archerly.models;

[Table("players")]
public class User: BaseModel
{
    [PrimaryKey("player_id")]
    public Guid Id { get; init; }
    [Column("user_id")]
    public Guid UserId { get; init; }
    public bool IsAdmin { get; init; }
    [Column("firstname")]
    public string FirstName { get; set; }
    [Column("lastname")]
    public string LastName { get; set; }
    [Column("nickname")]
    public string Nickname { get; set; }
}