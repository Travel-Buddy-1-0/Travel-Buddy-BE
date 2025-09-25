using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using TravelBuddyAPI.DTOs;
using static Supabase.Gotrue.Constants;

namespace TravelBuddyAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly Supabase.Client _client;
        private readonly ILogger<AuthenticationController> _logger;
        public AuthenticationController(Supabase.Client client, ILogger<AuthenticationController> logger)
        {
            _client = client;
            _logger = logger;   
        }
        [HttpPost("register")]
        public async Task<IResult> Register(RegisterRequestDto request)
        {
            try
            {
                var session = await _client.Auth.SignUp(request.Email, request.Password);
                return Results.Ok(session);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unable to register");

                // Return a Bad Request result with a specific problem detail
                return Results.BadRequest(new ProblemDetails
                {
                    Status = (int)HttpStatusCode.BadRequest,
                    Title = "Registration Failed",
                    Detail = "An error occurred while registering the user.",
                    Instance = Request.Path
                });
            }
        }
    }
}
