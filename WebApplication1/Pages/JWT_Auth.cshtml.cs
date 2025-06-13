using ConnectBackEnd;
using GlobalVariablesRP;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using WebApplication1.Model.Auth;
using WebApplication1.Model.Product.ProductDto;

namespace WebApplication1.Pages
{
    public class JWT_AuthModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory; // фабрика клиентов
        private readonly IConfiguration _configuration;

        public JWT_AuthModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        // Берём данные с формы авторизации
        [BindProperty]
        public AuthUserDto Input { get; set; }


        [BindProperty]
        public AuthUserDto LoginInput { get; set; }

        [BindProperty]
        public AddUserDto RegisterInput { get; set; }

        [BindProperty(SupportsGet = true)]
        public string ActiveTab { get; set; } = "login";

        public IActionResult OnGet()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                //если пользователь авторизован, то перекинем его сразу на его личный кабинет
                return RedirectToPage("/UserPages");
            }
            return null;
        }
        /// <summary>
        /// Обработка авторизации пользователя
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnPostAsync()
        {
            //if (!ModelState.IsValid)
            //{
            //    ActiveTab = "login";
            //    return Page(); 
            //}

            var client = _httpClientFactory.CreateClient();

            client.BaseAddress = new Uri(_configuration["ApiBaseUrl"]);

            var loginData = new
            {
                LoginInput.Login,
                LoginInput.Password
            };

           
            var response = await client.PostAsJsonAsync("gateway/auth/login", loginData);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadFromJsonAsync<AuthResponse>();

             
                // Сохраняем токен в куки и сессию
                Response.Cookies.Append("JWTToken", responseContent.Token, new CookieOptions
                {
                    HttpOnly = true, // Защита от XSS js не сможет работать с печенькой(
                    Secure = true, // Только HTTPS
                    SameSite = SameSiteMode.Strict, // Защита от CSRF
                    Expires = DateTimeOffset.Now.AddMinutes(30),
                    Path = "/" // Доступно для всех путей
                });
                // Не сохраняем токен в сессии - это избыточно при использовании кук
                // HttpContext.Session.SetString("JWTToken", responseContent.Token);

                // Перенаправляем на защищенную страницу
                return RedirectToPage("/UserPages");
            }
            
            ModelState.AddModelError(string.Empty, "Неверные учетные данные");
            ActiveTab = "login";
            return Page();
        }

        public async Task<IActionResult> OnPostRegisterAsync()
        {
            //if (!ModelState.IsValid)
            //{
            //    ActiveTab = "register";
            //  //  return Page();
            //}

            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["ApiBaseUrl"]);

            var registerData = new
            {
                RegisterInput.Name,
                RegisterInput.Surname,
                RegisterInput.Patronymic,
                RegisterInput.Login,
                RegisterInput.Password,
                RegisterInput.NumberPhone,
                RegisterInput.Email,
                RegisterInput.TelegramID
            };

            var response = await client.PostAsJsonAsync("gateway/auth/register", registerData);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Регистрация прошла успешно! Теперь вы можете войти в систему.";
                ActiveTab = "login";
                return RedirectToPage(new { tab = "login" });
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, $"Ошибка регистрации: {errorContent}");
            ActiveTab = "register";
            return Page();
        }

        private class AuthResponse
        {
            public string Token { get; set; }
            public int UserId { get; set; }
            public string Username { get; set; }
            public string Role { get; set; }
        }
        public class AddUserDto
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
}
