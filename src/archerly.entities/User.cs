using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace archerly.entities;

[Table("user")]
public class User : BaseModel
{
    // Matches the 'id uuid' primary key in your table
    [PrimaryKey("id")]
    public Guid Id { get; set; }

    [Column("isadmin")]
    public bool IsAdmin { get; set; }

    [Column("firstname")]
    public string FirstName { get; set; } = string.Empty;

    [Column("lastname")]
    public string LastName { get; set; } = string.Empty;

    [Column("nickname")]
    public string Nickname { get; set; } = string.Empty;
    [Column("highscore")]
    public long? HighScore { get; set; }

    // Empty user for defaults
    public static User Empty() => new User
    {
        Id = Guid.Empty,
        IsAdmin = false,
        FirstName = string.Empty,
        LastName = string.Empty,
        Nickname = string.Empty
    };

    // Factory method for creating a new user (no ID, let DB generate)
    public static User NewUser(string firstName, string lastName, string nickname, bool isAdmin) => new User
    {
        FirstName = firstName,
        LastName = lastName,
        Nickname = nickname,
        IsAdmin = isAdmin
    };

    // Factory method if you already have an ID (e.g., from Supabase Auth)
    public static User NewUserWithId(Guid id, string firstName, string lastName, string nickname, bool isAdmin) => new User
    {
        Id = id,
        FirstName = firstName,
        LastName = lastName,
        Nickname = nickname,
        IsAdmin = isAdmin
    };
}
