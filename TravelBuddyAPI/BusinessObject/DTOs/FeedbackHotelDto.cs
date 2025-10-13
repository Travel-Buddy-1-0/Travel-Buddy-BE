namespace BusinessObject.DTOs
{
    public class FeedbackHotelDto
    {
        public int FeedbackId { get; set; }
        public int UserId { get; set; }
        public int HotelId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UserName { get; set; }
        public string? HotelName { get; set; }
    }

    public class FeedbackHotelCreateRequestDto
    {
        public int UserId { get; set; }
        public int HotelId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
    }

    public class FeedbackHotelUpdateRequestDto
    {
        public int Rating { get; set; }
        public string? Comment { get; set; }
    }

    public class FeedbackHotelFilterRequestDto
    {
        public int? UserId { get; set; }
        public int? HotelId { get; set; }
        public int? Rating { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int Limit { get; set; } = 20;
        public int Offset { get; set; } = 0;
    }
}
