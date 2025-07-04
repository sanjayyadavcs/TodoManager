using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoManager.DTO;
using TodoManager.DTO.Requests;
using TodoManager.DTO.Responses;
using TodoManager.MVC.Helpers;
using TodoManager.Services.Interfaces;

namespace TodoManager.MVC.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger, IUserService userService)
        {
            _authService = authService;
            _logger = logger;
            _userService = userService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<LoginResponse>>> Login([FromBody] LoginRequest dto)
        {
            try
            {
                ApiResponse<LoginResponse> response = await _authService.AuthenticateAsync(dto);

                if (!response.Success)
                {
                    _logger.LogWarning("[AuthController][Login] Failed login attempt for user: {UserName}. Reason: {Message}", dto.UserName, response.Message);
                    return BadRequest(response);
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[AuthController][Login] Unexpected error during login for user: {UserName}", dto.UserName);
                return StatusCode(500, ApiResponse<string>.Fail("An unexpected error occurred during login. Please try again."));
            }
        }

        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse<string>>> Register([FromBody] RegisterRequest dto)
        {
            try
            {
                ApiResponse<string> response = await _authService.RegisterAsync(dto);

                if (!response.Success)
                {
                    _logger.LogError("[AuthController][Register] Registration failed for user: {UserName}. Reason: {Message}", dto.UserName, response.Message);
                    return BadRequest(response);
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[AuthController][Register] Unexpected error during registration for user: {UserName}", dto.UserName);
                return StatusCode(500, ApiResponse<string>.Fail("Registration failed due to a server issue. Please try again."));
            }
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult<ApiResponse<User>>> Me()
        {
            string username = this.ActingUserName();

            if (string.IsNullOrWhiteSpace(username))
            {
                _logger.LogError("[AuthController][Me] Acting username is null or empty.");
                return Unauthorized(ApiResponse<string>.Fail("You are not logged in. Please login to continue."));
            }

            try
            {
                User? user = await _userService.GetByUsernameAsync(username);

                if (user == null)
                {
                    _logger.LogError("[AuthController][Me] User not found for username: {UserName}", username);
                    return NotFound(ApiResponse<string>.Fail("User information could not be found."));
                }

                return Ok(ApiResponse<User>.Ok(user, "User profile loaded successfully."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[AuthController][Me] Failed to fetch user profile for username: {UserName}", username);
                return StatusCode(500, ApiResponse<string>.Fail("Unable to fetch your profile at the moment. Please try again."));
            }
        }
    }
}
