using BusinessLogic.Exceptions;
using BusinessLogic.Services;
using BusinessObject.DTOs;
using Microsoft.AspNetCore.Mvc;
using Supabase.Gotrue;

namespace TravelBuddyAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly Supabase.Client _client;
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUserProfile([FromBody] UpdateUserProfileRequest request)
        {
            try
            {
                await _client.Auth.SetSession(request.Auth.AccessToken, request.Auth.RefreshToken);

                var user = await _client.Auth.GetUser(request.Auth.AccessToken);
                if (user == null)
                    return Unauthorized("Invalid access token.");

                var updatedUser = await _userService.UpdateUserProfileAsync(user.Email, request.Profile);
                return Ok(updatedUser);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while updating the user profile.");
            }
        }


    }
}