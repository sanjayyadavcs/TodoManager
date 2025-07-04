using TodoManager.DAL.Entities;

namespace TodoManager.Services.Interfaces
{
    public interface ITokenService
    {
        Task<string> GenerateTokenAsync(User user, IList<string> roles);
        DateTime GetTokenExpiry();
    }
}
