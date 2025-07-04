using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace TodoManager.DAL.Entities
{
    public class User : IdentityUser<int>
    {
        [Required]
        public string FirstName { get; set; }

        [AllowNull]
        public string? LastName { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime UpdatedOn { get; set; } = DateTime.UtcNow;

        [AllowNull]
        public DateTime? DeletedOn { get; set; }

        public ICollection<UserRole> UserRoles { get; set; }
    }
}
