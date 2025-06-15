namespace AuthService.Dto
{
    public class AuthResponse
    {
        public string Token { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }
    }
}
