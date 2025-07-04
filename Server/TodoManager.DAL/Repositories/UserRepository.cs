using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using TodoManager.DAL.EF;
using TodoManager.DAL.Entities;
using TodoManager.DAL.Interfaces;

namespace TodoManager.DAL.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDBContext _context;
    private readonly ILogger<UserRepository> _logger;
    private readonly UserManager<User> _userManager;

    public UserRepository(ApplicationDBContext context, UserManager<User> userManager, ILogger<UserRepository> logger)
    {
        _context = context;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        try
        {
            // Includes UserRoles and corresponding Role
            return await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.UserName == username);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[UserRepository][GetByUsernameAsync] Error fetching user with username: {Username}", username);
            throw;
        }
    }
}
