namespace TodoManager.DTO
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Email { get; set; }
        public DateTime CreatedOn { get; set; }
        public List<Role> Roles { get; set; }
    }
}
