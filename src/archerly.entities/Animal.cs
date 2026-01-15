using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace archerly.entities;

[Supabase.Postgrest.Attributes.Table("animal")]
public class Animal : BaseModel
{
    [PrimaryKey("id")] public Guid Id { get; set; }

    // In Db this is currently Species
    [Column("species")] public string Name { get; set; }

    [Column("image_url")] public string ImageUrl { get; set; }

}