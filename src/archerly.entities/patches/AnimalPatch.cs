namespace archerly.entities.patches;

/// <summary>
/// Represents a partial update for an <see cref="Animal"/> entity.
/// <para>
/// Only the properties that need to be changed should be set; all other properties can be <c>null</c>.
/// Each property corresponds to the equivalent property on <see cref="Animal"/>.
/// This record is typically used in patch operations to avoid sending the full entity.
/// </para>
/// </summary>
/// <remarks>
/// Example usage:
/// <code>
/// var patch = new AnimalPatch(id: someId, species: "Lion", imageUrl: null);
/// </code>
/// In this example, only the <c>Species</c> property will be updated, leaving other fields unchanged.
/// </remarks>/ Its Fields should be equivalent although nullable with the Animal Entity
public record AnimalPatch
{
    public Guid Id { get; }
    public string? Species { get; }
    public string? ImageUrl { get; }
    public AnimalPatch(Guid id, string? species, string? imageUrl)
    {
        Id = id;
        Species = species;
        ImageUrl = imageUrl;
    }
}