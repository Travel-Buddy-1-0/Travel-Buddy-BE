using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    // --- LAYOUT CONFIG ---
    public class CvLayoutConfig
    {
        [JsonPropertyName("sectionsOrder")]
        public List<string> SectionsOrder { get; set; } = new();

        [JsonPropertyName("layout")]
        public LayoutDetails Layout { get; set; } = new();
    }

    public class LayoutDetails
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = "one-column"; // "one-column" | "two-column"
        [JsonPropertyName("left")]
        public List<string>? Left { get; set; }
        [JsonPropertyName("right")]
        public List<string>? Right { get; set; }
    }

    // --- STYLE CONFIG ---
    public class CvStyleConfig
    {
        [JsonPropertyName("global")]
        public Dictionary<string, string>? Global { get; set; }
        [JsonPropertyName("section")]
        public SectionStyle? Section { get; set; }
        [JsonPropertyName("avatar")]
        public Dictionary<string, string>? Avatar { get; set; }
        [JsonPropertyName("components")]
        public Dictionary<string, Dictionary<string, string>>? Components { get; set; }
    }

    public class SectionStyle
    {
        [JsonPropertyName("title")]
        public Dictionary<string, string>? Title { get; set; }
        [JsonPropertyName("content")]
        public Dictionary<string, string>? Content { get; set; }
    }

    // --- DATA JSON ---
    public class CvData
    {
        [JsonPropertyName("profile")]
        public ProfileData Profile { get; set; } = new();
        [JsonPropertyName("summary")]
        public string Summary { get; set; } = string.Empty;
        [JsonPropertyName("experience")]
        public List<ExperienceData> Experience { get; set; } = new();
        [JsonPropertyName("projects")]
        public List<ProjectData> Projects { get; set; } = new();
        [JsonPropertyName("education")]
        public List<EducationData> Education { get; set; } = new();
        [JsonPropertyName("skills")]
        public List<string> Skills { get; set; } = new();
        [JsonPropertyName("languages")]
        public List<string> Languages { get; set; } = new();
        [JsonPropertyName("interests")]
        public List<string> Interests { get; set; } = new();
    }

    public class ProfileData
    {
        [JsonPropertyName("avatar")]
        public string Avatar { get; set; } = string.Empty;
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;
        [JsonPropertyName("phone")]
        public string Phone { get; set; } = string.Empty;
    }

    public class ExperienceData
    {
        [JsonPropertyName("position")]
        public string Position { get; set; } = string.Empty;
        [JsonPropertyName("company")]
        public string Company { get; set; } = string.Empty;
        [JsonPropertyName("time")]
        public string Time { get; set; } = string.Empty;
        [JsonPropertyName("desc")]
        public string Desc { get; set; } = string.Empty;
    }

    public class ProjectData
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        [JsonPropertyName("tech")]
        public List<string> Tech { get; set; } = new();
        [JsonPropertyName("desc")]
        public string Desc { get; set; } = string.Empty;
    }

    public class EducationData
    {
        [JsonPropertyName("school")]
        public string School { get; set; } = string.Empty;
        [JsonPropertyName("year")]
        public string Year { get; set; } = string.Empty;
        [JsonPropertyName("degree")]
        public string Degree { get; set; } = string.Empty;
    }
}
