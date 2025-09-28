namespace BusinessObject.DTOs
{
    public class UserResponseDto
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public string? TokenType { get; set; }
        public long ExpiresIn { get; set; }
    }
}
