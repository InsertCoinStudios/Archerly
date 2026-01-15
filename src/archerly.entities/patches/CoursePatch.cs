namespace archerly.entities.patches;

/// <summary>
/// Represents a partial update for an <see cref="Course"/> entity.
/// <para>
/// Only the properties that need to be changed should be set; all other properties can be <c>null</c>.
/// Each property corresponds to the equivalent property on <see cref="Course"/>.
/// This record is typically used in patch operations to avoid sending the full entity.
/// </para>
/// </summary>
/// <remarks>
/// Example usage:
/// <code>
/// var patch = new CoursePatch(id: someId, name: "Forest", location: null, info: null, difficulty: null);
/// </code>
/// In this example, only the <c>Name</c> property will be updated, leaving other fields unchanged.
/// </remarks>/ Its Fields should be equivalent although nullable with the Course Entity
public record CoursePatch
{
    public Guid Id { get; }
    public string? Name { get; }
    public string? Location { get; }
    public string? Info { get; }
    public int? Difficulty { get; }

    public CoursePatch(string? name, string? location, string? info, int? difficulty)
    {
        Name = name;
        Location = location;
        Info = info;
        Difficulty = difficulty;
    }
}