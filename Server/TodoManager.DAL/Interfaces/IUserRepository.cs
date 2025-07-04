using TodoManager.DAL.Entities;

namespace TodoManager.DAL.Interfaces;

public interface IUserRepository
{
    Task<User> GetByUsernameAsync(string username);
}
