using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;

namespace BusinessObject.Models;

[Table("location")]
public class Location : BaseModel
{
    [PrimaryKey("location_id", false)]
    public int LocationId { get; set; }

    [Column("name")]
    public string Name { get; set; } = null!;

    [Column("description")]
    public string? Description { get; set; }

    [Column("latitude")]
    public decimal? Latitude { get; set; }

    [Column("longitude")]
    public decimal? Longitude { get; set; }

    [Column("country")]
    public string? Country { get; set; }

    [Column("city")]
    public string? City { get; set; }

    [Column("region")]
    public string? Region { get; set; }
}
