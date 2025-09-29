namespace BusinessObject.DTOs
{
    public class HotelFilterRequestDto
    {
        public string? Style { get; set; }
        public string? Location { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
