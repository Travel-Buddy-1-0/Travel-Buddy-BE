using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;
using System.Text.Json.Nodes;

namespace BusinessObject.Models;

[Table("userpreference")]
public class UserPreference : BaseModel
{
    [PrimaryKey("preference_id", false)]
    public int PreferenceId { get; set; }

    [Column("user_id")]
    public int? UserId { get; set; }

    [Column("travelstyle")]
    public JsonObject? TravelStyle { get; set; }

    [Column("budget")]
    public decimal? Budget { get; set; }

    [Column("cuisine")]
    public string? Cuisine { get; set; }

    [Column("preference_type")]
    public string? PreferenceType { get; set; }

    [Column("destionation")]
    public JsonObject? Destination { get; set; }
}
