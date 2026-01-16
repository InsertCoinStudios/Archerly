using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace archerly.entities;

[System.ComponentModel.DataAnnotations.Schema.Table("players")]
public class User : BaseModel
{
    [PrimaryKey("player_id")]
    public Guid Id { get; set; }
    // Users are identified using the auth.user.id from supabase
    // Note: does this link it to this table???
    [System.ComponentModel.DataAnnotations.Schema.Column("user_id")]
    public Guid UserId { get; set; }
    [System.ComponentModel.DataAnnotations.Schema.Column("is_admin")]
    public bool IsAdmin { get; set; }
    [System.ComponentModel.DataAnnotations.Schema.Column("firstname")]
    public string FirstName { get; set; }
    [System.ComponentModel.DataAnnotations.Schema.Column("lastname")]
    public string LastName { get; set; }
    [System.ComponentModel.DataAnnotations.Schema.Column("nickname")]
    public string Nickname { get; set; }

    public static User Empty()
    {
        var user = new User();
        user.Id = Guid.Empty;
        user.UserId = Guid.Empty;
        user.IsAdmin = false;
        user.FirstName = string.Empty;
        user.LastName = string.Empty;
        user.Nickname = string.Empty;
        return new User();
    }

    public static User NewUser(string firstName, string lastName, string nickname, bool isAdmin)
    {
        User user = new User();
        user.FirstName = firstName;
        user.LastName = lastName;
        user.IsAdmin = isAdmin;
        user.Nickname = nickname;
        return user;
    }


    public static User NewUserWithId(string id, string firstName, string lastName, string nickname, bool isAdmin)
    {
        if (!Guid.TryParse(id, out var guid))
        {
            throw new ArgumentException("Invalid user id format", nameof(id));
        }
        User user = new User();
        user.Id = guid;
        user.FirstName = firstName;
        user.LastName = lastName;
        user.IsAdmin = isAdmin;
        user.Nickname = nickname;
        return user;
    }
}