using Microsoft.AspNetCore.Identity;

namespace TodoManager.DAL.Entities
{
    public class Role : IdentityRole<int>
    {
        public Role() { }
        public Role(string role) : base(role) { }
        public ICollection<UserRole> UserRoles { get; set; }
    }

}
