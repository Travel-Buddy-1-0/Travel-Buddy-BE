using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class CvDataDto
    {
        [JsonPropertyName("profile")]
        public ProfileDto Profile { get; set; } = new();

        [JsonPropertyName("summary")]
        public string Summary { get; set; } = string.Empty;

        [JsonPropertyName("experience")]
        public List<ExperienceDto> Experience { get; set; } = new();

        [JsonPropertyName("projects")]
        public List<ProjectDto> Projects { get; set; } = new();

        [JsonPropertyName("education")]
        public List<EducationDto> Education { get; set; } = new();

        [JsonPropertyName("skills")]
        public List<string> Skills { get; set; } = new();

        [JsonPropertyName("languages")]
        public List<string> Languages { get; set; } = new();

        [JsonPropertyName("interests")]
        public List<string> Interests { get; set; } = new();
    }

    // Các class con
    public class ProfileDto
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

    public class ExperienceDto
    {
        [JsonPropertyName("position")]
        public string Position { get; set; } = string.Empty;

        [JsonPropertyName("company")]
        public string Company { get; set; } = string.Empty;

        [JsonPropertyName("time")]
        public string Time { get; set; } = string.Empty;

        [JsonPropertyName("desc")]
        public string Description { get; set; } = string.Empty; // Mapping "desc" sang "Description" cho rõ nghĩa
    }

    public class ProjectDto
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("tech")]
        public List<string> Technologies { get; set; } = new();

        [JsonPropertyName("desc")]
        public string Description { get; set; } = string.Empty;
    }

    public class EducationDto
    {
        [JsonPropertyName("school")]
        public string School { get; set; } = string.Empty;

        [JsonPropertyName("year")]
        public string Year { get; set; } = string.Empty;

        [JsonPropertyName("degree")]
        public string Degree { get; set; } = string.Empty;
    }
}
