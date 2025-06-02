using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Model
{
    public class LoginModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
