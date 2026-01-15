namespace archerly.entities.patches;

/// <summary>
/// Represents a partial update for an <see cref="User"/> entity.
/// <para>
/// Only the properties that need to be changed should be set; all other properties can be <c>null</c>.
/// Each property corresponds to the equivalent property on <see cref="User"/>.
/// This record is typically used in patch operations to avoid sending the full entity.
/// </para>
/// </summary>
/// Its Fields should be equivalent although nullable with the Course Entity
public record UserPatch
{
    public Guid Id { get; }
    public string? Firstname { get; }
    public string? Lastname { get; }
    public string? Nickname { get; }
    public UserPatch(Guid id, string? firstname, string? lastname, string? nickname)
    {
        Id = id;
        Firstname = firstname;
        Lastname = lastname;
        Nickname = nickname;
    }
}