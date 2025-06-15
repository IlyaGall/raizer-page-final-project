using System.ComponentModel.DataAnnotations;

namespace AuthService.Dto
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
