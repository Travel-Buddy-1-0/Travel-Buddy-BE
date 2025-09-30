using BusinessLogic.Exceptions;
using BusinessLogic.Services;
using BusinessObject.DTOs;
using Microsoft.AspNetCore.Mvc;
using Supabase;
using Supabase.Gotrue;
using System.Diagnostics;
using System.Net;
namespace TravelBuddyAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly Supabase.Client _client;
        private readonly ILogger<AuthenticationController> _logger;
        private readonly IUserService _userService;
        public AuthenticationController(Supabase.Client client, ILogger<AuthenticationController> logger, IUserService userService)
        {
            _client = client;
            _logger = logger;
            _userService = userService;
        }
        [HttpPost("register")]
        public async Task<IResult> Register([FromBody] RegisterRequestDto request)
        {
            try
            {
                var options = new SignUpOptions
                {
                    RedirectTo = "http://localhost:5173/success"
                };

                var session = await _client.Auth.SignUp(request.Email, request.Password, options);
                return Results.Ok(session);
            }
            catch (Supabase.Gotrue.Exceptions.GotrueException ex)
            {
                Console.WriteLine($"GoTrue Error: {ex.Message}");
                return Results.BadRequest(new { error = ex.Message });
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
        [HttpGet("getUser")]
        public async Task<IActionResult> GetUser([FromQuery] string accessToken)
        {
            try
            {
                if (string.IsNullOrEmpty(accessToken))
                    return BadRequest(new { error = "Missing access token" });
                var user = await _client.Auth.GetUser(accessToken);
                if (user == null)
                    return NotFound(new { error = "User not found" });
                var userModel = await _userService.GetUserByEmailAsync(user.Email);
                return Ok(new UserDto
                {
                    Email = userModel.Email,
                    FullName = userModel.FullName,
                    PhoneNumber = userModel.PhoneNumber,
                    DateOfBirth = userModel.DateOfBirth,
                    Sex = userModel.Sex,
                    Photo = userModel.Photo
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetUser");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }
        [HttpPost("confirmRegister")]
        public async Task<IActionResult> ConfirmRegister([FromBody] ConfirmRegisterRequestDto request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.AccessToken) || string.IsNullOrEmpty(request.RefreshToken))
                    return BadRequest(new { error = "Missing access or refresh token" });

                // Set lại session từ token
                await _client.Auth.SetSession(request.AccessToken, request.RefreshToken);

                // Lấy thông tin user
                var user1 = await _client.Auth.GetUser(request.AccessToken);
                if (user1 == null)
                    return BadRequest(new { error = "User not found" });

                var newUser = new BusinessObject.Entities.User
                {
                    Email = user1.Email,
                    RegistrationDate = DateTime.Now
                };

                var response = await _userService.CreateUserAsync(newUser);
                _logger.LogInformation($"User {user1.Email} registered and saved to DB.");
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ConfirmRegister");

                return BadRequest(new
                {
                    status = 400,
                    title = "Redirection Failed",
                    detail = ex.Message,
                    instance = Request.Path
                });
            }
        }



        [HttpPost("login")]
        public async Task<IResult> Login(RegisterRequestDto request)
        {
            try
            {
                var session = await _client.Auth.SignIn(request.Email, request.Password);
                if (session == null)
                {
                    return Results.BadRequest("Unable to login");
                }
                //var user = await _client.Auth.GetUser(session.AccessToken);
                var userResponse = new UserResponseDto
                {
                    AccessToken = session.AccessToken,
                    RefreshToken = session.RefreshToken,
                    ExpiresIn = session.ExpiresIn,
                    TokenType = session.TokenType
                };
                //await _client.Auth.SetSession(request.AccessToken, request.RefreshToken);
                return Results.Ok(userResponse);
            }
            catch (Supabase.Gotrue.Exceptions.GotrueException ex)
            {
                _logger.LogError($"Supabase Auth Error: {ex.Message}");
                return Results.BadRequest(new { error = ex.Message, code = ex.Response?.StatusCode });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unable to login: {ex.Message}");

                // Return a Bad Request result with a specific problem detail
                return Results.BadRequest(new ProblemDetails
                {
                    Status = (int)HttpStatusCode.BadRequest,
                    Title = "Login Failed",
                    Detail = "An error occurred while logging in the user.",
                    Instance = Request.Path
                });
            }
        }
        [HttpPost("resetpassword")]
        public async Task<IResult> ResetPassword(string email) 
        {
            try
            {
                // Gửi email reset password (Supabase sẽ dùng Site URL đã cấu hình)
                var success = await _client.Auth.ResetPasswordForEmail(email);

                if (!success)
                    return Results.BadRequest("Failed to send reset password email");

                return Results.Ok("Password reset email sent");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to send reset password email");

                return Results.BadRequest(new ProblemDetails
                {
                    Status = (int)HttpStatusCode.BadRequest,
                    Title = "Reset Password Failed",
                    Detail = "An error occurred while sending the reset password email.",
                    Instance = Request.Path
                });
            }
        }

        [HttpPost("updatepassword")]
        public async Task<IResult> UpdatePassword([FromBody] UpdatePasswordDto request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.AccessToken) || string.IsNullOrEmpty(request.NewPassword))
                    return Results.BadRequest("Missing access token or new password");

                // Set session từ access token để xác thực user
                await _client.Auth.SetSession(request.AccessToken, request.RefreshToken);

                // Cập nhật mật khẩu
                var user = await _client.Auth.Update(new UserAttributes
                {
                    Password = request.NewPassword
                });

                return Results.Ok("Password updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to update password");

                return Results.BadRequest(new ProblemDetails
                {
                    Status = (int)HttpStatusCode.BadRequest,
                    Title = "Update Password Failed",
                    Detail = "An error occurred while updating the password.",
                    Instance = Request.Path
                });
            }
        }
        private string HashPassword(string password)
        {
            // Ví dụ đơn giản dùng SHA256 (hoặc dùng Identity/KeyDerivation)
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var bytes = System.Text.Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        [HttpPost("logout")]
        public async Task<IResult> Logout(RegisterRequestDto request)
        {
            try
            {
                await _client.Auth.SignOut();
                return Results.Ok("Logged out");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unable to logout" +
                    $"");

                // Return a Bad Request result with a specific problem detail
                return Results.BadRequest(new ProblemDetails
                {
                    Status = (int)HttpStatusCode.BadRequest,
                    Title = "Login Failed",
                    Detail = "An error occurred while logging in the user.",
                    Instance = Request.Path
                });
            }
        }
        [HttpGet("login-google")]
        public async Task<IActionResult> LoginWithGoogle()
        {
            try
            {
                var options = new SignInOptions
                {
                    RedirectTo = "http://localhost:5173/Authentication/oauth-callback" // cái này Hưng sử url để gọi về google-session truyền 2 cái token vào là được
                };

                // Lấy URL để redirect user sang Google login
                var url = _client.Auth.SignIn(Constants.Provider.Google, options);
                return Ok(url.Result.Uri);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Google Login");
                return BadRequest(new ProblemDetails
                {
                    Status = (int)HttpStatusCode.BadRequest,
                    Title = "Google Login Failed",
                    Detail = "An error occurred while starting Google login.",
                    Instance = Request.Path
                });
            }
        }

            [HttpPost("google-session")]
            public async Task<IActionResult> GoogleSession([FromBody] UserResponseDto dto)
            {
                try
                {
                Debug.WriteLine("Received AccessToken: {AccessToken}", dto.AccessToken);
                Debug.WriteLine("Received RefreshToken: {RefreshToken}", dto.RefreshToken);

                await _client.Auth.SetSession(dto.AccessToken, dto.RefreshToken);
                    var user = await _client.Auth.GetUser(dto.AccessToken);

                    if (user == null)
                        return BadRequest(new { error = "User not found" });
                BusinessObject.Entities.User? userModel = null;
                try
                {
                    userModel = await _userService.GetUserByEmailAsync(user.Email);
                }
                catch (NotFoundException)
                {
                    var newUser = new BusinessObject.Entities.User
                    {
                        Email = user.Email,
                    };
                    await _userService.CreateUserAsync(newUser);
                }
                    return Ok(new
                    {
                        Email = user.Email
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in GoogleSession");
                    return BadRequest(new { error = "Google session error" });
                }
            }
    }
}
