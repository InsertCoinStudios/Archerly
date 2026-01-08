using System.Text.Json.Serialization;
using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;
namespace archerly.models;

[Supabase.Postgrest.Attributes.Table("animal")]
public class Animal : BaseModel
{
    [PrimaryKey("id")] public Guid Id { get; set; }

    // In Db this is currently Species
    [Column("species")] public string Name { get; set; }

    [Column("image_url")] public string ImageUrl { get; set; }

}