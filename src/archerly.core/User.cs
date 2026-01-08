namespace archerly.core;

public class User
{
    public Guid Id { get; init; }
    public long UserId { get; init; }
    public bool IsAdmin { get; init; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Nickname { get; set; }

    public User(Guid Id, long UserId, bool IsAdmin, string FirstName, string LastName, string Nickname)
    {
        this.Id = Id;
        this.UserId = UserId;
        this.IsAdmin = IsAdmin;
        this.FirstName = FirstName;
        this.LastName = LastName;
        this.Nickname = Nickname;
    }
}