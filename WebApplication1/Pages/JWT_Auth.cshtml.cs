using ConnectBackEnd;
using GlobalVariablesRP;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using AuthService.Dto;


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
            var client = _httpClientFactory.CreateClient();

            client.BaseAddress = new Uri(_configuration["ApiBaseUrl"]);

            var loginData = new
            {
                // берём данные с формы
                LoginInput.Login,
                LoginInput.Password
            };

            try
            {
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
                        Expires = DateTime.UtcNow.AddMinutes(69),
                        Path = "/" // Доступно для всех путей
                    });
                    // Не сохраняем токен в сессии - это избыточно при использовании кук
                    // HttpContext.Session.SetString("JWTToken", responseContent.Token);

                    // Перенаправляем на защищенную страницу (личная страница пользователя)
                    return RedirectToPage("/UserPages");
                }
                else 
                {
                    TempData["SuccessMessage"] = "Кажется, кто-то пытается взломать матрицу... но у вас просто опечатка! 😉\r\n\r\nПопробуй ещё раз — вдруг это злобный глюк съел букву?";
                }
            }
            catch (HttpRequestException ex) 
            {
                TempData["SuccessMessage"] = "Фиксик перебрал с электролитами, но наши доктора уже его чинят! 🔌💉\r\nСервис авторизации ненадолго отключился — даём слово, скоро всё заработает! 😉";
            }
            catch (Exception ex)
            {
                    TempData["SuccessMessage"] = ex.ToString();

            }//  ModelState.AddModelError(string.Empty, "Неверные учетные данные");
         //   ActiveTab = "login";
            return Page();
        }

        /// <summary>
        /// Регистрация нового пользователя
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnPostRegisterAsync()
        {

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
     
    }
}
