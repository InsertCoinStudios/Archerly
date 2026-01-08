namespace archerly.core;

public class Animal
{
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