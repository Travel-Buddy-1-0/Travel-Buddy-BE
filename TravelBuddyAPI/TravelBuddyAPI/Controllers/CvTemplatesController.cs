using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace TravelBuddyAPI.Controllers
{
    [Route("api/cv-templates")]
    [ApiController]
    public class CvTemplatesController : ControllerBase
    {
        private readonly ICvService _cvService;

        public CvTemplatesController(ICvService cvService)
        {
            _cvService = cvService;
        }

        [HttpGet]
        public async Task<IActionResult> GetTemplates()
        {
            var result = await _cvService.GetAllTemplatesAsync();
            return Ok(result);
        }
    }
}
