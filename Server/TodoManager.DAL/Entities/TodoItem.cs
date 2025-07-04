using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using TodoManager.DAL.Enums;

namespace TodoManager.DAL.Entities
{
    [Table("TodoItems")]
    public class TodoItem : BaseEntity
    {
        [Required]
        public string Title { get; set; }

        public string? Description { get; set; }

        [Required]
        public TodoCategory Category { get; set; }

        [Required]
        public TodoPriority Priority { get; set; }

        [Required]
        public bool IsCompleted { get; set; } = false;

        public DateTime? CompletedAt { get; set; }

        public DateTime? DueDate { get; set; }

        [Required]
        public User Owner { get; set; }

        public static DTO.TodoItem ToDTO(TodoItem item) => new()
        {
            Id = item.Id,
            Title = item.Title,
            Description = item.Description,
            Priority = item.Priority.ToString(),
            Category = item.Category.ToString(),
            IsCompleted = item.IsCompleted,
            CreatedOn = item.CreatedOn,
        };

    }
}
