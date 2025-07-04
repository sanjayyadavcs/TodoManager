using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TodoManager.DAL.Entities;
using TodoManager.DTO.Requests;
using TodoManager.DTO.Responses;
using TodoManager.Services.Interfaces;

namespace TodoManager.Services.ServiceImplementation
{
    public class AuthService : IAuthService
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _config;
        private readonly ILogger<AuthService> _logger;
        private readonly UserManager<User> _userManager;

        public AuthService(IUserService userService, ITokenService  tokenService, IConfiguration config, ILogger<AuthService> logger, UserManager<User> userManager)
        {
            _userService = userService;
            _tokenService = tokenService;
            _config = config;
            _logger = logger;
            _userManager = userManager;
        }

        public async Task<ApiResponse<string>> RegisterAsync(RegisterRequest dto)
        {
            try
            {
                if (dto.Password != dto.ConfirmPassword)
                {
                    return ApiResponse<string>.Fail("Passwords do not match.");
                }

                var existingUser = await _userManager.FindByNameAsync(dto.UserName);
                if (existingUser != null)
                {
                    return ApiResponse<string>.Fail("Username is already taken.");
                }

                var user = new User
                {
                    UserName = dto.UserName,
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Email = dto.Email
                };

                var result = await _userManager.CreateAsync(user, dto.Password);

                if (!result.Succeeded)
                {
                    var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                    return ApiResponse<string>.Fail($"Registration failed: {errors}");
                }

                await _userManager.AddToRoleAsync(user, "User");

                return ApiResponse<string>.Ok(null, "User registered successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while registering user: {Username}", dto.UserName);
                return ApiResponse<string>.Fail("An internal error occurred during registration.");
            }
        }

        public async Task<ApiResponse<LoginResponse>> AuthenticateAsync(LoginRequest dto)
        {
            try
            {
                User? user = await _userManager.FindByNameAsync(dto.UserName);

                if (user == null)
                {
                    _logger.LogWarning("Login failed: User not found: {Username}", dto.UserName);
                    return ApiResponse<LoginResponse>.Fail("Invalid username or password.");
                }

                var isPasswordValid = await _userManager.CheckPasswordAsync(user, dto.Password);
                if (!isPasswordValid)
                {
                    _logger.LogWarning("Login failed: Invalid password for user: {Username}", dto.UserName);
                    return ApiResponse<LoginResponse>.Fail("Invalid username or password.");
                }

                var roles = await _userManager.GetRolesAsync(user);
                string token = await _tokenService.GenerateTokenAsync(user, roles);

                var response = new LoginResponse
                {
                    Token = token,
                    ExpiresAt = _tokenService.GetTokenExpiry(),
                    User = await _userService.GetByUsernameAsync(user.UserName)
                };

                return ApiResponse<LoginResponse>.Ok(response, "Login successful.");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login error for user: {Username}", dto.UserName);
                return ApiResponse<LoginResponse>.Fail("An internal error occurred during login.");
            }
        }

    }
}
