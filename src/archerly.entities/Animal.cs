using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace archerly.entities;

//[Table("animal")]
public class Animal
//: BaseModel
{
    //    [PrimaryKey("id")]
    public Guid Id { get; set; }

    //    [Column("name")]
    public string Name { get; set; } = string.Empty;

    //    [Column("imageurl")]
    public string ImageUrl { get; set; } = string.Empty;

    public static Animal NewAnimal(string name, string imageUrl)
    {
        var animal = new Animal();
        animal.Name = name;
        animal.ImageUrl = imageUrl;
        return animal;
    }

    public static Animal NewAnimalWithId(string id, string name, string imageUrl)
    {
        if (!Guid.TryParse(id, out var guid))
        {
            throw new ArgumentException("Invalid user id format", nameof(id));
        }
        var animal = new Animal();
        animal.Id = guid;
        animal.Name = name;
        animal.ImageUrl = imageUrl;
        return animal;
    }

    public static Animal NewAnimalWithId(Guid id, string name, string imageUrl)
    {
        var animal = new Animal();
        animal.Id = id;
        animal.Name = name;
        animal.ImageUrl = imageUrl;
        return animal;
    }
}