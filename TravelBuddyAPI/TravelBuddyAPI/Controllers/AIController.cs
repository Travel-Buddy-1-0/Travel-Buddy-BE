using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mscc.GenerativeAI;
namespace TravelBuddyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AIController : ControllerBase
    {
        private string GetKey()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", true, true).Build();
            return configuration["Gemini:Key"];
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
            var googleAI = new GoogleAI();  // or pass in apiKey parameter
            var model = googleAI.GenerativeModel(model: Model.Gemini25Flash);

            var response = await model.GenerateContent(searchText);
            return Ok(response);
        }
    }
}
