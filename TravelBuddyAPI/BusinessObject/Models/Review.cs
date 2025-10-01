using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;
using Newtonsoft.Json.Linq;

namespace BusinessObject.Models;

[Table("reviews")]
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
    public JToken? Image { get; set; }

    [Column("review_date")]
    public DateTime? ReviewDate { get; set; }
}
