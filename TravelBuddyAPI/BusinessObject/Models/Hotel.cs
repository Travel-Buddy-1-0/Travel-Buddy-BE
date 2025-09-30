using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;
using Newtonsoft.Json.Linq;

namespace BusinessObject.Models;

[Table("hotel")]
public class Hotel : BaseModel
{
    [PrimaryKey("hotel_id", false)]
    public int HotelId { get; set; }

    [Column("name")]
    public string Name { get; set; } = null!;

    [Column("description")]
    public string? Description { get; set; }

    [Column("address")]
    public string? Address { get; set; }

    [Column("image")]
    public JToken? Image { get; set; }

    [Column("style")]
    public JToken? Style { get; set; }
}
