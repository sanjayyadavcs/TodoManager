using TodoManager.DTO.Requests;
using TodoManager.DTO.Responses;

namespace TodoManager.Services.Interfaces
{
    public interface IAuthService
    {
        Task<ApiResponse<LoginResponse>> AuthenticateAsync(LoginRequest dto);
        Task<ApiResponse<string>> RegisterAsync(RegisterRequest dto);
    }
}
