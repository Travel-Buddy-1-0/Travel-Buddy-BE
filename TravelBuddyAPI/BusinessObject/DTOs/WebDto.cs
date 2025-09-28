using System.Text.Json.Nodes;

namespace BusinessObject.DTOs
{
    public class UserDto
    {
        public string? Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? FullName { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string? Photo { get; set; }
        public string Role { get; set; } = null!;
        public string? Sex { get; set; }
    }

    public class UpdatePasswordDto
    {
        public string AccessToken { get; set; } = null!;
        public string RefreshToken { get; set; } = null;
        public string NewPassword { get; set; } = null!;
    }
    public class ConversationDto
    {
        public int ConversationId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public List<MessageDto>? Messages { get; set; }
        public List<ParticipantDto>? Participants { get; set; }
    }

    public class GroupDto
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public string? Description { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class HotelDto
    {
        public int HotelId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? Address { get; set; }
        public string? Image { get; set; }
        public string? Style { get; set; }
    }

    public class LocationDto
    {
        public int LocationId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
        public string? Region { get; set; }
    }

    public class MessageDto
    {
        public int MessageId { get; set; }
        public int ConversationId { get; set; }
        public int SenderId { get; set; }
        public string Content { get; set; }
        public DateTime? SentAt { get; set; }
        public bool? IsRead { get; set; }
    }

    public class ParticipantDto
    {
        public int ParticipantId { get; set; }
        public int ConversationId { get; set; }
        public int UserId { get; set; }
        public DateTime? JoinedAt { get; set; }
    }

    public class RestaurantDto
    {
        public int RestaurantId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal? AveragePrice { get; set; }
        public string? Image { get; set; }
        public string? Style { get; set; }
        public string? Address { get; set; }
    }

    public class ReviewDto
    {
        public int ReviewId { get; set; }
        public int? UserId { get; set; }
        public int? TourId { get; set; }
        public int? HotelId { get; set; }
        public int? RestaurantId { get; set; }
        public int? Rating { get; set; }
        public string? Comment { get; set; }
        public string? Image { get; set; }
        public DateTime? ReviewDate { get; set; }
    }

    public class RoomDto
    {
        public int RoomId { get; set; }
        public int? HotelId { get; set; }
        public string RoomNumber { get; set; }
        public string RoomType { get; set; }
        public decimal PricePerNight { get; set; }
        public int Capacity { get; set; }
        public bool? IsAvailable { get; set; }
        public string? Image { get; set; }
    }

    public class ServiceDto
    {
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public string? Type { get; set; }
        public int? ProviderId { get; set; }
    }

    public class TourDto
    {
        public int TourId { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public int? Duration { get; set; }
        public decimal? Price { get; set; }
        public int? LocationId { get; set; }
    }

    public class UserActivityDto
    {
        public int ActivityId { get; set; }
        public int? UserId { get; set; }
        public string? ActivityType { get; set; }
        public DateTime? ActivityDate { get; set; }
        public string? Metadata { get; set; }
    }

    public class UserPreferenceDto
    {
        public int PreferenceId { get; set; }
        public int? UserId { get; set; }
        public string? TravelStyle { get; set; }
        public decimal? Budget { get; set; }
        public string? Cuisine { get; set; }
        public string? PreferenceType { get; set; }
        public string? Destination { get; set; }
    }
}
