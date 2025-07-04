using Microsoft.EntityFrameworkCore;
using TodoManager.DAL.EF;
using TodoManager.DAL.Entities;
using TodoManager.DAL.Enums;
using TodoManager.DAL.Interfaces;
using Microsoft.Extensions.Logging;

namespace TodoManager.DAL.Repositories;

public class TodoRepository : ITodoRepository
{
    private readonly ApplicationDBContext _context;
    private readonly ILogger<TodoRepository> _logger;

    public TodoRepository(ApplicationDBContext context, ILogger<TodoRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<TodoItem>> GetAllByUsernameAsync(string username)
    {
        try
        {
            return await _context.TodoItems
                .Include(t => t.Owner)
                .Where(t => t.Owner.UserName == username)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[TodoRepository][GetAllByUsernameAsync] Failed to fetch todos for user: {Username}", username);
            throw;
        }
    }

    public async Task<IEnumerable<TodoItem>> GetTodosWithFiltersAsync(string username, string? search, string? category, string? priority, string? sort)
    {
        try
        {
            var query = _context.TodoItems
                .Include(t => t.Owner)
                .AsQueryable()
                .Where(t => t.Owner.UserName == username);

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(t =>
                    t.Title.Contains(search) ||
                    (t.Description != null && t.Description.Contains(search))
                );
            }

            // Parse category if provided
            if (!string.IsNullOrWhiteSpace(category) && category.ToLower() != "all")
            {
                query = query.Where(t => t.Category == Enum.Parse<TodoCategory>(category, true));
            }

            // Parse priority safely
            if (!string.IsNullOrWhiteSpace(priority) &&
                Enum.TryParse<TodoPriority>(priority, true, out var parsedPriority))
            {
                query = query.Where(t => t.Priority == parsedPriority);
            }

            // Apply sorting
            query = sort switch
            {
                "created_asc" => query.OrderBy(t => t.CreatedOn),
                "created_desc" => query.OrderByDescending(t => t.CreatedOn),
                "priority_desc" => query.OrderByDescending(t => t.Priority),
                "title_asc" => query.OrderBy(t => t.Title),
                _ => query.OrderByDescending(t => t.CreatedOn)
            };

            return await query.ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[TodoRepository][GetTodosWithFiltersAsync] Failed to filter todos for user: {Username}", username);
            throw;
        }
    }

    public async Task<TodoItem?> GetByIdAndUsernameAsync(int id, string username)
    {
        try
        {
            return await _context.TodoItems
                .Include(t => t.Owner)
                .FirstOrDefaultAsync(t => t.Id == id && t.Owner.UserName == username);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[TodoRepository][GetByIdAndUsernameAsync] Failed to fetch todo ID: {Id} for user: {Username}", id, username);
            throw;
        }
    }

    public async Task<IEnumerable<TodoItem>> SearchAsync(string query, string username)
    {
        try
        {
            return await _context.TodoItems
                .Include(t => t.Owner)
                .Where(t =>
                    t.Owner.UserName == username &&
                    (t.Title.Contains(query) || (t.Description != null && t.Description.Contains(query)))
                )
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[TodoRepository][SearchAsync] Failed to search todos with query '{Query}' for user: {Username}", query, username);
            throw;
        }
    }

    public async Task<IEnumerable<TodoItem>> GetByCategoryAsync(string categoryName, string username)
    {
        try
        {
            return await _context.TodoItems
                .Include(t => t.Owner)
                .Where(t => t.Owner.UserName == username && t.Category == Enum.Parse<TodoCategory>(categoryName, true))
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[TodoRepository][GetByCategoryAsync] Failed to fetch category '{Category}' for user: {Username}", categoryName, username);
            throw;
        }
    }

    public async Task<TodoItem> CreateAsync(TodoItem item)
    {
        try
        {
            _context.TodoItems.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[TodoRepository][CreateAsync] Failed to create todo for user: {Username}", item.Owner?.UserName);
            throw;
        }
    }

    public async Task<TodoItem?> UpdateAsync(int id, TodoItem updated, string username)
    {
        try
        {
            var existing = await GetByIdAndUsernameAsync(id, username);
            if (existing == null) return null;

            existing.Title = updated.Title;
            existing.Description = updated.Description;
            existing.Priority = updated.Priority;
            existing.Category = updated.Category;
            existing.IsCompleted = updated.IsCompleted;
            existing.CompletedAt = updated.CompletedAt;
            existing.DueDate = updated.DueDate;

            await _context.SaveChangesAsync();
            return existing;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[TodoRepository][UpdateAsync] Failed to update todo ID: {Id} for user: {Username}", id, username);
            throw;
        }
    }

    public async Task<TodoItem?> ToggleCompletionAsync(int id, string username)
    {
        try
        {
            var todo = await GetByIdAndUsernameAsync(id, username);
            if (todo == null) return null;

            // Flip completion state and adjust timestamps
            todo.IsCompleted = !todo.IsCompleted;
            todo.CompletedAt = todo.IsCompleted ? DateTime.UtcNow : null;
            todo.UpdatedOn = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return todo;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[TodoRepository][ToggleCompletionAsync] Failed to toggle completion for ID: {Id}", id);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(int id, string username)
    {
        try
        {
            var item = await GetByIdAndUsernameAsync(id, username);
            if (item == null) return false;

            _context.TodoItems.Remove(item);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[TodoRepository][DeleteAsync] Failed to delete todo ID: {Id} for user: {Username}", id, username);
            throw;
        }
    }

    public async Task<DTO.TodoStats> GetStaticStatsAsync(string username)
    {
        try
        {
            var todos = await _context.TodoItems
                .Where(t => t.Owner.UserName == username)
                .ToListAsync();

            return new DTO.TodoStats
            {
                Total = todos.Count,
                Work = todos.Count(t => t.Category == TodoCategory.Work),
                Personal = todos.Count(t => t.Category == TodoCategory.Personal),
                Completed = todos.Count(t => t.IsCompleted),
                HighPriority = todos.Count(t => t.Priority == TodoPriority.High),
                MediumPriority = todos.Count(t => t.Priority == TodoPriority.Medium),
                LowPriority = todos.Count(t => t.Priority == TodoPriority.Low)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[TodoRepository][GetStaticStatsAsync] Failed to calculate stats for user: {Username}", username);
            throw;
        }
    }
}
