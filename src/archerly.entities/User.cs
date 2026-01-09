using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace archerly.entities;

[System.ComponentModel.DataAnnotations.Schema.Table("players")]
public class User: BaseModel
{
    [PrimaryKey("player_id")]
    public Guid Id { get; init; }
    [System.ComponentModel.DataAnnotations.Schema.Column("user_id")]
    public Guid UserId { get; init; }
    public bool IsAdmin { get; init; }
    [System.ComponentModel.DataAnnotations.Schema.Column("firstname")]
    public string FirstName { get; set; }
    [System.ComponentModel.DataAnnotations.Schema.Column("lastname")]
    public string LastName { get; set; }
    [System.ComponentModel.DataAnnotations.Schema.Column("nickname")]
    public string Nickname { get; set; }
}