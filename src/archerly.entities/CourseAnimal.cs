using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace archerly.entities;
[Table("course_animals")]
public class CourseAnimal:BaseModel
{
    [PrimaryKey("id")]
    public Guid Id { get; init; }
    [Column("course_id")]
    public Guid CourseId { get; init; }
    [Column("animal_id")]
    public Guid AnimalId { get; init; }
    [Column("order_number")]
    public string OrderNumber { get; init; }
}