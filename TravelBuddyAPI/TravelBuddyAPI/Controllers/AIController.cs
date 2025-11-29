using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mscc.GenerativeAI;
using Newtonsoft.Json;
using Services.Interfaces;
using TravelBuddyAPI.Models;
namespace TravelBuddyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AIController : ControllerBase
    {
        private readonly IFileParserService _fileParserService;
        private readonly IConfiguration _configuration;
        public AIController(IConfiguration configuration, IFileParserService fileParserService)
        {
            _configuration = configuration;
            _fileParserService = fileParserService;
        }

        private string GetKey()
        {
            //IConfiguration configuration = new ConfigurationBuilder()
            //        .SetBasePath(Directory.GetCurrentDirectory())
            //        .AddJsonFile("appsettings.json", true, true).Build();
            //IConfiguration configuration = builder.Configuration.Get<Config>() ?? new Config();
            var key = _configuration["Gemini:Key"];
            return key;
        }
        private string GetUrl()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", true, true).Build();
            return configuration["Gemini:Url"] + GetKey();
        }
        [HttpPost("generate-text")]
        public async Task<IActionResult> GetAIBaseResult(string searchText)
        {
            var apiKey = GetKey();
            var googleAI = new GoogleAI(apiKey);
            var model = googleAI.GenerativeModel(model: Model.Gemini25Flash);

            var response = await model.GenerateContent(searchText);
            return Ok(response);
        }

        [HttpPost("gen-cv")]
        public async Task<IActionResult> GenCV(string searchText)
        {

            var input = JsonConvert.SerializeObject(searchText);
            var apiKey = GetKey();
            var googleAI = new GoogleAI(apiKey);
            var model = googleAI.GenerativeModel(model: Model.Gemini25Flash);
            //var generationConfig = new GenerationConfig() 
            //{
            //    ResponseMimeType = "application/json",
            //    ResponseSchema = 
            //};

            var response = await model.GenerateContent(searchText);
            return Ok(response);
        }

        [HttpPost("upload-cv")]
        public async Task<IActionResult> UploadCv(IFormFile file)
        {
            using var stream = file.OpenReadStream();

            var text = await _fileParserService.ExtractTextAsync(stream, file.FileName);

            var prompt = $@"
                Bạn là một trợ lý AI chuyên trích xuất dữ liệu CV.
                Nhiệm vụ: Trích xuất thông tin từ văn bản thô bên dưới và trả về định dạng JSON CHÍNH XÁC theo cấu trúc tôi cung cấp.
                
                Yêu cầu:
                1. Chỉ trả về JSON thuần túy, không có Markdown (```json), không có lời dẫn.
                2. Nếu thông tin không có, hãy để chuỗi rỗng hoặc mảng rỗng, đừng bịa ra.
                3. Dịch các nội dung mô tả sang tiếng Việt nếu văn bản gốc là tiếng Anh (hoặc giữ nguyên tùy bạn chọn).

                Cấu trúc JSON mong muốn:
                {{
                    ""profile"": {{ ""avatar"": ""string"", ""name"": ""string"", ""email"": ""string"", ""phone"": ""string"" }},
                    ""summary"": ""string"",
                    ""experience"": [ {{ ""position"": ""string"", ""company"": ""string"", ""time"": ""string"", ""desc"": ""string"" }} ],
                    ""projects"": [ {{ ""name"": ""string"", ""tech"": [""string""], ""desc"": ""string"" }} ],
                    ""education"": [ {{ ""school"": ""string"", ""year"": ""string"", ""degree"": ""string"" }} ],
                    ""skills"": [""string""],
                    ""languages"": [""string""],
                    ""interests"": [""string""]
                }}

                Văn bản thô từ CV:
                {text}";
            var apiKey = GetKey();
            var googleAI = new GoogleAI(apiKey);
            var model = googleAI.GenerativeModel(model: Model.Gemini25Flash);
            var generationConfig = new GenerationConfig()
            {
                ResponseMimeType = "application/json",
                ResponseSchema = CreateCvSchema()
            };

            var response = await model.GenerateContent(prompt, generationConfig: generationConfig);

            return Ok(response);
        }

        private Schema CreateCvSchema()
        {
            var cvSchema = new Schema
            {
                Type = ParameterType.Object,
                Description = "Cấu trúc dữ liệu CV đầy đủ",
                Properties = new Dictionary<string, Schema>
        {
            // 1. PROFILE (Object)
            { "profile", new Schema
                {
                    Type = ParameterType.Object,
                    Description = "Thông tin cá nhân",
                    Properties = new Dictionary<string, Schema>
                    {
                        { "avatar", new Schema { Type = ParameterType.String, Description = "Link ảnh đại diện" } },
                        { "name", new Schema { Type = ParameterType.String, Description = "Họ và tên đầy đủ" } },
                        { "email", new Schema { Type = ParameterType.String, Description = "Địa chỉ email" } },
                        { "phone", new Schema { Type = ParameterType.String, Description = "Số điện thoại" } }
                    },
                    Required = new List<string> { "name", "email" } // Ví dụ bắt buộc phải có tên, email
                }
            },

            // 2. SUMMARY (String)
            { "summary", new Schema
                {
                    Type = ParameterType.String,
                    Description = "Đoạn văn giới thiệu ngắn gọn"
                }
            },

            // 3. EXPERIENCE (Array of Objects)
            { "experience", new Schema
                {
                    Type = ParameterType.Array,
                    Description = "Kinh nghiệm làm việc",
                    Items = new Schema
                    {
                        Type = ParameterType.Object,
                        Properties = new Dictionary<string, Schema>
                        {
                            { "position", new Schema { Type = ParameterType.String } },
                            { "company", new Schema { Type = ParameterType.String } },
                            { "time", new Schema { Type = ParameterType.String } },
                            { "desc", new Schema { Type = ParameterType.String, Description = "Mô tả chi tiết công việc" } }
                        },
                        Required = new List<string> { "position", "company" }
                    }
                }
            },

            // 4. PROJECTS (Array of Objects)
            { "projects", new Schema
                {
                    Type = ParameterType.Array,
                    Description = "Các dự án cá nhân hoặc công ty",
                    Items = new Schema
                    {
                        Type = ParameterType.Object,
                        Properties = new Dictionary<string, Schema>
                        {
                            { "name", new Schema { Type = ParameterType.String } },
                            // Mảng String bên trong Object
                            { "tech", new Schema
                                {
                                    Type = ParameterType.Array,
                                    Items = new Schema { Type = ParameterType.String },
                                    Description = "Danh sách công nghệ sử dụng"
                                }
                            },
                            { "desc", new Schema { Type = ParameterType.String } }
                        }
                    }
                }
            },

            // 5. EDUCATION (Array of Objects)
            { "education", new Schema
                {
                    Type = ParameterType.Array,
                    Description = "Học vấn",
                    Items = new Schema
                    {
                        Type = ParameterType.Object,
                        Properties = new Dictionary<string, Schema>
                        {
                            { "school", new Schema { Type = ParameterType.String } },
                            { "year", new Schema { Type = ParameterType.String } },
                            { "degree", new Schema { Type = ParameterType.String } }
                        }
                    }
                }
            },

            // 6. SKILLS (Array of Strings)
            { "skills", new Schema
                {
                    Type = ParameterType.Array,
                    Description = "Danh sách kỹ năng chuyên môn",
                    Items = new Schema { Type = ParameterType.String }
                }
            },

            // 7. LANGUAGES (Array of Strings)
            { "languages", new Schema
                {
                    Type = ParameterType.Array,
                    Description = "Ngôn ngữ",
                    Items = new Schema { Type = ParameterType.String }
                }
            },

            // 8. INTERESTS (Array of Strings)
            { "interests", new Schema
                {
                    Type = ParameterType.Array,
                    Description = "Sở thích cá nhân",
                    Items = new Schema { Type = ParameterType.String }
                }
            }
        },
                // Danh sách các trường bắt buộc ở cấp cao nhất
                Required = new List<string> { "profile", "experience", "education", "skills" }
            };

            return cvSchema;
        }
    }
}
