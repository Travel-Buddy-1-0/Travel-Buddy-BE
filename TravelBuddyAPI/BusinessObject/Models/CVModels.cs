using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
   
        // Root Object
        public class CvGenerationRequest
        {
            [JsonPropertyName("layout_json")]
            public LayoutJson LayoutJson { get; set; }

            [JsonPropertyName("style_json")]
            public StyleJson StyleJson { get; set; }

            [JsonPropertyName("data_json")]
            public DataJson DataJson { get; set; }
        }

        // --- LAYOUT ---
        public class LayoutJson
        {
            [JsonPropertyName("sectionsOrder")]
            public List<string> SectionsOrder { get; set; }

            [JsonPropertyName("layout")]
            public LayoutConfig Layout { get; set; }
        }

        public class LayoutConfig
        {
            [JsonPropertyName("type")]
            public string Type { get; set; } // "one-column" | "two-column"

            [JsonPropertyName("left")]
            public List<string>? Left { get; set; }

            [JsonPropertyName("right")]
            public List<string>? Right { get; set; }
        }

        // --- STYLE ---
        // Sử dụng Dictionary để map với Record<string, string> của TS
        public class StyleJson
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

        //public class SectionStyle
        //{
        //    [JsonPropertyName("title")]
        //    public Dictionary<string, string>? Title { get; set; }

        //    [JsonPropertyName("content")]
        //    public Dictionary<string, string>? Content { get; set; }
        //}

        // --- DATA ---
        public class DataJson
        {
            [JsonPropertyName("profile")]
            public ProfileData Profile { get; set; }

            [JsonPropertyName("summary")]
            public string Summary { get; set; }

            [JsonPropertyName("experience")]
            public List<ExpData> Experience { get; set; }

            [JsonPropertyName("projects")]
            public List<ProjectData> Projects { get; set; }

            [JsonPropertyName("education")]
            public List<EduData> Education { get; set; }

            [JsonPropertyName("skills")]
            public List<string> Skills { get; set; }

            [JsonPropertyName("languages")]
            public List<string> Languages { get; set; }

            [JsonPropertyName("interests")]
            public List<string> Interests { get; set; }
        }

        //public class ProfileData
        //{
        //    [JsonPropertyName("avatar")]
        //    public string Avatar { get; set; }
        //    [JsonPropertyName("name")]
        //    public string Name { get; set; }
        //    [JsonPropertyName("email")]
        //    public string Email { get; set; }
        //    [JsonPropertyName("phone")]
        //    public string Phone { get; set; }
        //}

        public class ExpData
        {
            [JsonPropertyName("position")]
            public string Position { get; set; }
            [JsonPropertyName("company")]
            public string Company { get; set; }
            [JsonPropertyName("time")]
            public string Time { get; set; }
            [JsonPropertyName("desc")]
            public string Desc { get; set; }
        }

        //public class ProjectData
        //{
        //    [JsonPropertyName("name")]
        //    public string Name { get; set; }
        //    [JsonPropertyName("tech")]
        //    public List<string> Tech { get; set; }
        //    [JsonPropertyName("desc")]
        //    public string Desc { get; set; }
        //}

        public class EduData
        {
            [JsonPropertyName("school")]
            public string School { get; set; }
            [JsonPropertyName("year")]
            public string Year { get; set; }
            [JsonPropertyName("degree")]
            public string Degree { get; set; }
        }
    
}
