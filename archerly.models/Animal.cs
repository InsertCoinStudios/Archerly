using System.Text.Json.Serialization;

namespace archerly.models;

public class Animal
{
    [JsonPropertyName("id")]
    public Guid Id { get; init; }

    // In Db this is currently Species
    public string Name { get; set; }
    public string ImageUrl { get; set; }

    public Animal(Guid id, string name, string imageUrl)
    {
        Id = id;
        Name = name;
        ImageUrl = imageUrl;
    }
}