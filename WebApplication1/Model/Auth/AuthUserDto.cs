using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Model.Auth
{
    public class AuthUserDto
    {
        [Required]
        public string Login { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
