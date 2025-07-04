using TodoManager.DAL.Entities;

namespace TodoManager.DAL.Interfaces
{
    public interface ITodoRepository
    {
        Task<IEnumerable<TodoItem>> GetAllByUsernameAsync(string username);
        Task<IEnumerable<TodoItem>> GetTodosWithFiltersAsync(string username, string? search, string? category, string? priority, string? sort);
        Task<TodoItem?> GetByIdAndUsernameAsync(int id, string username);
        Task<IEnumerable<TodoItem>> SearchAsync(string query, string username);
        Task<IEnumerable<TodoItem>> GetByCategoryAsync(string categoryName, string username);
        Task<TodoItem> CreateAsync(TodoItem item);
        Task<TodoItem?> UpdateAsync(int id, TodoItem item, string username);
        Task<TodoItem?> ToggleCompletionAsync(int id, string username);
        Task<bool> DeleteAsync(int id, string username);
        Task<DTO.TodoStats> GetStaticStatsAsync(string username);
    }
}
