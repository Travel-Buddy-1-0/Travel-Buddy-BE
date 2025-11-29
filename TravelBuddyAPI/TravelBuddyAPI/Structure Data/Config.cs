namespace TravelBuddyAPI.Models
{
    public class Config
    {
        public string Provider { get; set; } = "Gemini";
        public GeminiConfig Gemini { get; set; } = new GeminiConfig();
    }

    public class GeminiConfig
    {
        public string Key { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
    }
}
