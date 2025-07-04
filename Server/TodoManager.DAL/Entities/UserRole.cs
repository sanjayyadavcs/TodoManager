using Microsoft.AspNetCore.Identity;

namespace TodoManager.DAL.Entities
{
    public class UserRole : IdentityUserRole<int>
    {
        public User User { get; set; }
        public Role Role { get; set; }
    }

}
