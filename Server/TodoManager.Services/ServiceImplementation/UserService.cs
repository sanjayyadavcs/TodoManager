using Microsoft.Extensions.Logging;
using TodoManager.DAL.Interfaces;
using TodoManager.DTO;
using TodoManager.Services.Interfaces;

namespace TodoManager.Services.ServiceImplementation
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            try
            {
                var user = await _userRepository.GetByUsernameAsync(username);
                if (user == null)
                {
                    _logger.LogWarning("[UserService][GetByUsernameAsync] User not found: {Username}", username);
                    return null;
                }

                return new User
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    UserName = user.UserName,
                    Email = user.Email,
                    CreatedOn = user.CreatedOn,
                    Roles = user.UserRoles
                        .Select(role => new Role
                        {
                            Id = role.RoleId,
                            Name = role.Role.Name
                        })
                        .ToList()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[UserService][GetByUsernameAsync] Failed to fetch user with username: {Username}", username);
                throw;
            }
        }
    }
}
