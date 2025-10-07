using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;
using Newtonsoft.Json.Linq;

namespace BusinessObject.Models;

[Table("rooms")]
public class Room : BaseModel
{
    [PrimaryKey("room_id", false)]
    public int RoomId { get; set; }

    [Column("hotel_id")]
    public int? HotelId { get; set; }

    [Column("room_number")]
    public string RoomNumber { get; set; } = null!;

    [Column("room_type")]
    public string RoomType { get; set; } = null!;

    [Column("price_per_night")]
    public decimal PricePerNight { get; set; }

    [Column("capacity")]
    public int Capacity { get; set; }

    [Column("is_available")]
    public bool? IsAvailable { get; set; }

    [Column("image")]
    public JToken? Image { get; set; }
}
