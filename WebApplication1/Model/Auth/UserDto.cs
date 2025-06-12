namespace WebApplication1.Model.Auth
{

    namespace AuthService.BLL.Dto
    {
        public class UserDto
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

            /// <summary>
            /// Пароль пользователя
            /// </summary>
            public string Password { get; set; } = string.Empty;



            /// <summary>
            /// Мобильный телефон пользователя
            /// </summary>
            public string NumberPhone { get; set; } = string.Empty;



            /// <summary>
            /// Почта пользователя
            /// </summary>
            public string Email { get; set; } = string.Empty;


            public long TelegramID { get; set; }

        }
    }

}
