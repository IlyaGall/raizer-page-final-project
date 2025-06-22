
namespace AuthService.BLL.Dto
{
    public class UserEasyDto
    {
        /// <summary>
        /// Id пользователя
        /// </summary>
        public int Id { get; set; }


        /// <summary>
        /// Логин пользователя
        /// </summary>
        public string Login { get; set; } = string.Empty;


        /// <summary>
        /// Имя пользователя
        /// </summary>
        public string Name { get; set; } = string.Empty;


        /// <summary>
        /// Фамилия пользователя
        /// </summary>
        public string Surname { get; set; } = string.Empty;


        /// <summary>
        /// Отчество
        /// </summary>
        public string Patronymic { get; set; } = string.Empty;
    }
}
