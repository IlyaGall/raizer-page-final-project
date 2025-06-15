using System.ComponentModel.DataAnnotations;

namespace AuthService.Dto
{
    public class UpdateDateDto
    {
        /// <summary>
        /// Имя пользователя
        /// </summary>
        [Required(ErrorMessage = "Имя обязательно")]
        [StringLength(50, ErrorMessage = "Имя не должно превышать 50 символов")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Фамилия пользователя
        /// </summary>
        [Required(ErrorMessage = "Фамилия обязательна")]
        [StringLength(50, ErrorMessage = "Фамилия не должна превышать 50 символов")]
        public string Surname { get; set; } = string.Empty;

        /// <summary>
        /// Отчество
        /// </summary>
        [StringLength(50, ErrorMessage = "Отчество не должно превышать 50 символов")]
        public string Patronymic { get; set; } = string.Empty;

        /// <summary>
        /// Логин
        /// </summary>
        [Required(ErrorMessage = "Логин обязателен")]
        [StringLength(20, MinimumLength = 4, ErrorMessage = "Логин должен быть от 4 до 20 символов")]
        public string Login { get; set; } = string.Empty;

        /// <summary>
        /// Пароль пользователя
        /// </summary>
        [Required(ErrorMessage = "Пароль обязателен")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Пароль должен быть от 6 до 100 символов")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Мобильный телефон пользователя
        /// </summary>
        [Phone(ErrorMessage = "Неверный формат телефона")]
        public string NumberPhone { get; set; } = string.Empty;

        /// <summary>
        /// Почта пользователя
        /// </summary>
        [EmailAddress(ErrorMessage = "Неверный формат email")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Индефикатор пользователя
        /// </summary>
        [Range(1, long.MaxValue, ErrorMessage = "Telegram ID должен быть положительным числом")]
        public long TelegramID { get; set; }
    }
}
