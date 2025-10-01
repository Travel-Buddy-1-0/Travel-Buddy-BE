using Newtonsoft.Json.Linq;

namespace BusinessObject.DTOs
{
    public class RoomDetailDto
    {
        public int RoomId { get; set; }
        public int? HotelId { get; set; }
        public string RoomNumber { get; set; } = null!;
        public string RoomType { get; set; } = null!;
        public decimal PricePerNight { get; set; }
        public int Capacity { get; set; }
        public bool? IsAvailable { get; set; }
        public JToken? Image { get; set; }
        public DateOnly? LastCheckoutDate { get; set; }
        public HotelSummaryDto? Hotel { get; set; }
    }
}
