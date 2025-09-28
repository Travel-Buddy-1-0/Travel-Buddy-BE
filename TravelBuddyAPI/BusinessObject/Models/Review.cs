using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;
using System.Text.Json.Nodes;

namespace BusinessObject.Models;

[Table("review")]
public class Review : BaseModel
{
    [PrimaryKey("review_id", false)]
    public int ReviewId { get; set; }

    [Column("user_id")]
    public int? UserId { get; set; }

    [Column("tour_id")]
    public int? TourId { get; set; }

    [Column("hotel_id")]
    public int? HotelId { get; set; }

    [Column("restaurant_id")]
    public int? RestaurantId { get; set; }

    [Column("rating")]
    public int? Rating { get; set; }

    [Column("comment")]
    public string? Comment { get; set; }

    [Column("image")]
    public JsonObject? Image { get; set; }

    [Column("review_date")]
    public DateTime? ReviewDate { get; set; }
}
