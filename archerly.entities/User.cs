using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace archerly.models;

[Table("players")]
public class User: BaseModel
{
    [PrimaryKey("player_id")]
    public Guid Id { get; init; }
    [Column("user_id")]
    public long UserId { get; init; }
    public bool IsAdmin { get; init; }
    [Column("firstname")]
    public string FirstName { get; set; }
    [Column("lastname")]
    public string LastName { get; set; }
    [Column("nickname")]
    public string Nickname { get; set; }
    public string Email { get; set; }

    public User(Guid Id, long UserId, bool IsAdmin, string FirstName, string LastName, string Nickname)
    {
        this.Id = Id;
        this.UserId = UserId;
        this.IsAdmin = IsAdmin;
        this.FirstName = FirstName;
        this.LastName = LastName;
        this.Nickname = Nickname;
    }

    public User()
    {
        throw new NotImplementedException();
    }
}