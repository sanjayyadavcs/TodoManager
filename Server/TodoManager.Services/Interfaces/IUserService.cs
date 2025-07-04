using TodoManager.DTO;

namespace TodoManager.Services.Interfaces;

public interface IUserService
{
    Task<User?> GetByUsernameAsync(string username);
}
