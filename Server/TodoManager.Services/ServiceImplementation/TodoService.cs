using TodoManager.DAL.Enums;
using TodoManager.DAL.Interfaces;
using TodoManager.DTO;
using TodoManager.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace TodoManager.Services.ServiceImplementation;

public class TodoService : ITodoService
{
    private readonly ITodoRepository _repo;
    private readonly IUserRepository _userRepo;
    private readonly ILogger<TodoService> _logger;

    public TodoService(ITodoRepository repo, IUserRepository userRepo, ILogger<TodoService> logger)
    {
        _repo = repo;
        _userRepo = userRepo;
        _logger = logger;
    }

    private static TodoItem ToDTO(DAL.Entities.TodoItem item) => new()
    {
        Id = item.Id,
        Title = item.Title,
        Description = item.Description,
        Priority = item.Priority.ToString(),
        Category = item.Category.ToString(),
        IsCompleted = item.IsCompleted,
        CreatedOn = item.CreatedOn,
        CompletedAt = item.CompletedAt,
        DueDate = item.DueDate
    };

    public async Task<IEnumerable<TodoItem>> GetAllAsync(string username)
    {
        try
        {
            var items = await _repo.GetAllByUsernameAsync(username);
            return items.Select(ToDTO);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[TodoService][GetAllAsync] Failed to fetch tasks for user: {Username}", username);
            return Enumerable.Empty<TodoItem>();
        }
    }

    public async Task<IEnumerable<TodoItem>> GetFilteredTodosAsync(string username, string? search, string? category, string? priority, string? sort)
    {
        try
        {
            var items = await _repo.GetTodosWithFiltersAsync(username, search, category, priority, sort);
            return items.Select(ToDTO);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[TodoService][GetFilteredTodosAsync] Error filtering tasks for user: {Username}", username);
            return Enumerable.Empty<TodoItem>();
        }
    }

    public async Task<TodoItem?> GetByIdAsync(int id, string username)
    {
        try
        {
            var item = await _repo.GetByIdAndUsernameAsync(id, username);
            return item == null ? null : ToDTO(item);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[TodoService][GetByIdAsync] Failed to get task ID: {Id} for user: {Username}", id, username);
            throw;
        }
    }

    public async Task<IEnumerable<TodoItem>> SearchAsync(string query, string username)
    {
        try
        {
            var results = await _repo.SearchAsync(query, username);
            return results.Select(ToDTO);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[TodoService][SearchAsync] Error during search with query '{Query}' for user: {Username}", query, username);
            return Enumerable.Empty<TodoItem>();
        }
    }

    public async Task<IEnumerable<TodoItem>> GetByCategoryAsync(string category, string username)
    {
        try
        {
            var items = await _repo.GetByCategoryAsync(category, username);
            return items.Select(ToDTO);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[TodoService][GetByCategoryAsync] Failed to get tasks for category '{Category}' and user: {Username}", category, username);
            throw;
        }
    }

    public async Task<TodoItem> CreateAsync(string username, TodoItem dto)
    {
        try
        {
            var user = await _userRepo.GetByUsernameAsync(username);
            var entity = new DAL.Entities.TodoItem
            {
                Title = dto.Title,
                Description = dto.Description,
                Priority = Enum.Parse<TodoPriority>(dto.Priority, true),
                Category = Enum.Parse<TodoCategory>(dto.Category, true),
                IsCompleted = dto.IsCompleted,
                CreatedOn = dto.CreatedOn ?? DateTime.UtcNow,
                CompletedAt = dto.CompletedAt,
                DueDate = dto.DueDate,
                Owner = user
            };

            var result = await _repo.CreateAsync(entity);
            return ToDTO(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[TodoService][CreateAsync] Failed to create task for user: {Username}", username);
            throw;
        }
    }

    public async Task<TodoItem?> UpdateAsync(int id, string username, TodoItem dto)
    {
        try
        {
            var updated = new DAL.Entities.TodoItem
            {
                Id = id,
                Title = dto.Title,
                Description = dto.Description,
                Priority = Enum.Parse<TodoPriority>(dto.Priority, true),
                Category = Enum.Parse<TodoCategory>(dto.Category, true),
                IsCompleted = dto.IsCompleted,
                CreatedOn = dto.CreatedOn ?? DateTime.UtcNow,
                CompletedAt = dto.CompletedAt,
                DueDate = dto.DueDate
            };

            var result = await _repo.UpdateAsync(id, updated, username);
            return result == null ? null : ToDTO(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[TodoService][UpdateAsync] Failed to update task ID: {Id} for user: {Username}", id, username);
            throw;
        }
    }

    public async Task<TodoItem?> ToggleCompletionAsync(int id, string username)
    {
        try
        {
            var item = await _repo.ToggleCompletionAsync(id, username);
            return item == null ? null : ToDTO(item);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[TodoService][ToggleCompletionAsync] Failed to toggle task ID: {Id} for user: {Username}", id, username);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(int id, string username)
    {
        try
        {
            return await _repo.DeleteAsync(id, username);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[TodoService][DeleteAsync] Failed to delete task ID: {Id} for user: {Username}", id, username);
            throw;
        }
    }

    public async Task<TodoStats> GetStaticStatsAsync(string username)
    {
        try
        {
            return await _repo.GetStaticStatsAsync(username);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[TodoService][GetStaticStatsAsync] Failed to fetch stats for user: {Username}", username);
            throw;
        }
    }
}
