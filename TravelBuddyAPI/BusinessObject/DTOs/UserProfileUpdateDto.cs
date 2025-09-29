namespace BusinessObject.DTOs
{
    public class UserProfileUpdateDto
    {
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Image { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string? Sex { get; set; }
    }
    public class UpdateUserProfileRequest
    {
        public AuthRequestDto Auth { get; set; }
        public UserProfileUpdateDto Profile { get; set; }
    }
}
