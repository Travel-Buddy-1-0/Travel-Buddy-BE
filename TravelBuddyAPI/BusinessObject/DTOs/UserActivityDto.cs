namespace BusinessObject.DTOs
{
    public class UserActivityDto
    {
        public int ActivityId { get; set; }
        public int? UserId { get; set; }
        public string? ActivityType { get; set; }
        public DateTime? ActivityDate { get; set; }
        public string? Metadata { get; set; }
    }

    public class UserActivityCreateRequestDto
    {
        public int? UserId { get; set; }
        public string? ActivityType { get; set; }
        public DateTime? ActivityDate { get; set; }
        public string? Metadata { get; set; }
    }

    public class UserActivityUpdateRequestDto
    {
        public int? UserId { get; set; }
        public string? ActivityType { get; set; }
        public DateTime? ActivityDate { get; set; }
        public string? Metadata { get; set; }
    }

    public class UserActivityFilterRequestDto
    {
        public int? UserId { get; set; }
        public string? ActivityType { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int Limit { get; set; } = 20;
        public int Offset { get; set; } = 0;
    }
}
