using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoManager.DTO;
using TodoManager.DTO.Responses;
using TodoManager.MVC.Helpers;
using TodoManager.Services.Interfaces;

namespace TodoManager.MVC.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TodoController : ControllerBase
{
    private readonly ITodoService _service;
    private readonly ILogger<TodoController> _logger;

    public TodoController(ITodoService service, ILogger<TodoController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<TodoItem>>>> GetAll(
        [FromQuery] string? search,
        [FromQuery] string? category,
        [FromQuery] string? priority,
        [FromQuery] string? sort)
    {
        string username = this.ActingUserName();
        try
        {
            IEnumerable<TodoItem> todos = await _service.GetFilteredTodosAsync(username, search, category, priority, sort);
            return Ok(ApiResponse<IEnumerable<TodoItem>>.Ok(todos, "Todos loaded successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[TodoController][GetAll] Failed to load todos. User: {User}", username);
            return StatusCode(500, ApiResponse<string>.Fail("Something went wrong while loading your tasks. Please try again."));
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<TodoItem>>> GetById(int id)
    {
        string username = this.ActingUserName();
        try
        {
            TodoItem? item = await _service.GetByIdAsync(id, username);
            return item == null
                ? NotFound(ApiResponse<TodoItem>.Fail("Task not found."))
                : Ok(ApiResponse<TodoItem>.Ok(item, "Task retrieved successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[TodoController][GetById] Failed to fetch task ID: {Id}. User: {User}", id, username);
            return StatusCode(500, ApiResponse<string>.Fail("Could not retrieve the task. Please try again later."));
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<TodoItem>>> Create([FromBody] TodoItem dto)
    {
        string username = this.ActingUserName();
        try
        {
            TodoItem created = await _service.CreateAsync(username, dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, ApiResponse<TodoItem>.Ok(created, "Task created successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[TodoController][Create] Failed to create task. User: {User}", username);
            return StatusCode(500, ApiResponse<string>.Fail("Unable to create the task right now. Please try again."));
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<TodoItem>>> Update(int id, [FromBody] TodoItem dto)
    {
        string username = this.ActingUserName();
        try
        {
            TodoItem? updated = await _service.UpdateAsync(id, username, dto);
            return updated == null
                ? NotFound(ApiResponse<TodoItem>.Fail("Task not found."))
                : Ok(ApiResponse<TodoItem>.Ok(updated, "Task updated successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[TodoController][Update] Failed to update task ID: {Id}. User: {User}", id, username);
            return StatusCode(500, ApiResponse<string>.Fail("Could not update the task. Please try again."));
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<string>>> Delete(int id)
    {
        string username = this.ActingUserName();
        try
        {
            bool success = await _service.DeleteAsync(id, username);
            return success
                ? Ok(ApiResponse<string>.Ok(null, "Task deleted successfully."))
                : NotFound(ApiResponse<string>.Fail("Task not found."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[TodoController][Delete] Failed to delete task ID: {Id}. User: {User}", id, username);
            return StatusCode(500, ApiResponse<string>.Fail("Could not delete the task. Please try again later."));
        }
    }

    [HttpPatch("{id}/toggle")]
    public async Task<ActionResult<ApiResponse<TodoItem>>> ToggleCompletion(int id)
    {
        string username = this.ActingUserName();
        if (id <= 0)
            return BadRequest(ApiResponse<string>.Fail("Invalid task ID."));

        try
        {
            TodoItem? todo = await _service.ToggleCompletionAsync(id, username);
            return todo == null
                ? NotFound(ApiResponse<string>.Fail("Task not found."))
                : Ok(ApiResponse<TodoItem>.Ok(todo, "Task status updated."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[TodoController][ToggleCompletion] Failed to toggle task ID: {Id}. User: {User}", id, username);
            return StatusCode(500, ApiResponse<string>.Fail("Could not update task status. Try again shortly."));
        }
    }

    [HttpGet("search")]
    public async Task<ActionResult<ApiResponse<IEnumerable<TodoItem>>>> Search([FromQuery] string query)
    {
        string username = this.ActingUserName();
        try
        {
            IEnumerable<TodoItem> results = await _service.SearchAsync(query, username);
            return Ok(ApiResponse<IEnumerable<TodoItem>>.Ok(results, "Search completed successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[TodoController][Search] Failed to search with query: {Query}. User: {User}", query, username);
            return StatusCode(500, ApiResponse<string>.Fail("Could not complete the search. Please try again."));
        }
    }

    [HttpGet("category/{categoryName}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<TodoItem>>>> GetByCategory(string categoryName)
    {
        string username = this.ActingUserName();
        if (string.IsNullOrWhiteSpace(categoryName))
            return BadRequest(ApiResponse<string>.Fail("Category name is required."));

        try
        {
            IEnumerable<TodoItem> items = await _service.GetByCategoryAsync(categoryName, username);
            return Ok(ApiResponse<IEnumerable<TodoItem>>.Ok(items, $"Tasks in '{categoryName}' loaded."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[TodoController][GetByCategory] Failed to fetch category: {Category}. User: {User}", categoryName, username);
            return StatusCode(500, ApiResponse<string>.Fail("Could not load tasks from that category."));
        }
    }

    [HttpGet("stats")]
    public async Task<ActionResult<ApiResponse<TodoStats>>> GetTodoStats()
    {
        string username = this.ActingUserName();
        try
        {
            TodoStats stats = await _service.GetStaticStatsAsync(username);
            return Ok(ApiResponse<TodoStats>.Ok(stats, "Task statistics loaded."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[TodoController][GetTodoStats] Failed to load stats. User: {User}", username);
            return StatusCode(500, ApiResponse<string>.Fail("Could not load task stats. Try again later."));
        }
    }
}
