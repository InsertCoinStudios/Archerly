using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace archerly.entities;
[System.ComponentModel.DataAnnotations.Schema.Table("course_animals")]
public class CourseAnimal:BaseModel
{
    [PrimaryKey("id")]
    public Guid Id { get; set; }
    [Column("course_id")]
    public int CourseId { get; set; }
    [Column("animal_id")]
    public int AnimalId { get; set; }
    [Column("order_number")]
    public string OrderNumber { get; set; }
}