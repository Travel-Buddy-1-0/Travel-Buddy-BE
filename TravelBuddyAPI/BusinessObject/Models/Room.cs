using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;
using System.Text.Json.Nodes;

namespace BusinessObject.Models;

[Table("room")]
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
    public JsonObject? Image { get; set; }
}
