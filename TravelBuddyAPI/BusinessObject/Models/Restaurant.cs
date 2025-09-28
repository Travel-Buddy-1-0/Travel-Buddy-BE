using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;
using System.Text.Json.Nodes;

namespace BusinessObject.Models;

[Table("restaurant")]
public class Restaurant : BaseModel
{
    [PrimaryKey("restaurant_id", false)]
    public int RestaurantId { get; set; }

    [Column("name")]
    public string Name { get; set; } = null!;

    [Column("description")]
    public string? Description { get; set; }

    [Column("average_price")]
    public decimal? AveragePrice { get; set; }

    [Column("image")]
    public JsonObject? Image { get; set; }

    [Column("style")]
    public JsonObject? Style { get; set; }

    [Column("address")]
    public string? Address { get; set; }
}
