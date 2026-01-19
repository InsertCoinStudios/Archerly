using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace archerly.entities;

[System.ComponentModel.DataAnnotations.Schema.Table("user")]
public class User : BaseModel
{
    [PrimaryKey("id")]
    public Guid SupaId { get; init; }
    // Users are identified using the auth.user.id from supabase
    // Note: does this link it to this table???
    [System.ComponentModel.DataAnnotations.Schema.Column("userid")]
    public Guid UserId { get; set; }
    [System.ComponentModel.DataAnnotations.Schema.Column("isadmin")]
    public bool IsAdmin { get; set; }
    [System.ComponentModel.DataAnnotations.Schema.Column("firstname")]
    public string FirstName { get; set; }
    [System.ComponentModel.DataAnnotations.Schema.Column("lastname")]
    public string LastName { get; set; }
    [System.ComponentModel.DataAnnotations.Schema.Column("nickname")]
    public string Nickname { get; set; }
    
}