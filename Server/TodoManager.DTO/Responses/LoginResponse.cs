namespace TodoManager.DTO.Responses
{
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public User? User { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
