using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;

namespace BusinessObject.Models;

[Table("tour")]
public class Tour : BaseModel
{
    [PrimaryKey("tour_id", false)]
    public int TourId { get; set; }

    [Column("title")]
    public string Title { get; set; } = null!;

    [Column("description")]
    public string? Description { get; set; }

    [Column("duration")]
    public int? Duration { get; set; }

    [Column("price")]
    public decimal? Price { get; set; }

    [Column("location_id")]
    public int? LocationId { get; set; }
}
