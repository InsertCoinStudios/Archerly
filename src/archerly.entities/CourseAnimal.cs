using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace archerly.entities;
[Table("coursexanimals")]
public class CourseAnimal:BaseModel
{
    [Column("course_id")]
    public Guid CourseId { get; init; }
    [Column("animal_id")]
    public Guid AnimalId { get; init; }
    [Column("order")]
    public string Order { get; init; }
}