using System.Text.Json.Serialization;

namespace TravelBuddyAPI.DTOs
{
    public class ConfirmRegisterRequestDto
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }
        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; }
        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("expires_at")]
        public string ExpiresAt { get; set; }
        [JsonPropertyName("expires_in")]
        public string ExpiresIn { get; set; }
    }
}
