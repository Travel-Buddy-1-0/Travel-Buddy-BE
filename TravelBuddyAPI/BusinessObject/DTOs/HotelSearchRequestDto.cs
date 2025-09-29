namespace BusinessObject.DTOs
{
    public class HotelSearchRequestDto
    {
        public string? Location { get; set; }
        public DateTime? CheckIn { get; set; }
        public DateTime? CheckOut { get; set; }
        public int? Guests { get; set; }
        public string? Style { get; set; }
    }
}
