using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace archerly.entities;

//[Table("coursexanimal")]
public class CourseAnimal
//: BaseModel
{
    //   [PrimaryKey("courseid")]
    public Guid CourseId { get; set; }

    //    [PrimaryKey("animalid")]
    public Guid AnimalId { get; set; }

    //    [Column("order")]
    public long Order { get; set; }
}