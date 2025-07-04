using TodoManager.DTO;

namespace TodoManager.Services.Interfaces
{
    public interface ITodoService
    {
        Task<IEnumerable<TodoItem>> GetAllAsync(string username);
        Task<IEnumerable<TodoItem>> GetFilteredTodosAsync(string username, string? search, string? category, string? priority, string? sort);
        Task<TodoItem?> GetByIdAsync(int id, string username);
        Task<IEnumerable<TodoItem>> SearchAsync(string query, string username);
        Task<IEnumerable<TodoItem>> GetByCategoryAsync(string category, string username);
        Task<TodoItem> CreateAsync(string username, TodoItem item);
        Task<TodoItem?> UpdateAsync(int id, string username, TodoItem item);
        Task<TodoItem?> ToggleCompletionAsync(int id, string username);
        Task<bool> DeleteAsync(int id, string username);
        Task<TodoStats> GetStaticStatsAsync(string username);
    }
}
