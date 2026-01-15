using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace archerly.entities;

[System.ComponentModel.DataAnnotations.Schema.Table("players")]
public class User : BaseModel
{
    [PrimaryKey("player_id")]
    public Guid Id { get; init; }
    // Users are identified using the auth.user.id from supabase
    // Note: does this link it to this table???
    [System.ComponentModel.DataAnnotations.Schema.Column("user_id")]
    public Guid UserId { get; init; }
    [System.ComponentModel.DataAnnotations.Schema.Column("is_admin")]
    public bool IsAdmin { get; init; }
    [System.ComponentModel.DataAnnotations.Schema.Column("firstname")]
    public string FirstName { get; set; }
    [System.ComponentModel.DataAnnotations.Schema.Column("lastname")]
    public string LastName { get; set; }
    [System.ComponentModel.DataAnnotations.Schema.Column("nickname")]
    public string Nickname { get; set; }
}