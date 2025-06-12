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

        // ЅерЄм данные с формы авторизации
        [BindProperty]
        public AuthUserDto Input { get; set; }

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
        /// ќбработка авторизации пользовател€
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnPostAsync()
        {

            

            if (!ModelState.IsValid)
            { 
                return Page(); 
            }

            var client = _httpClientFactory.CreateClient();

            client.BaseAddress = new Uri(_configuration["ApiBaseUrl"]);

            var loginData = new
            {
                Input.Login,
                Input.Password
            };

           
            var response = await client.PostAsJsonAsync("gateway/auth/login", loginData);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadFromJsonAsync<AuthResponse>();

             
                // —охран€ем токен в куки и сессию
                Response.Cookies.Append("JWTToken", responseContent.Token, new CookieOptions
                {
                    HttpOnly = true, // «ащита от XSS js не сможет работать с печенькой(
                    Secure = true, // “олько HTTPS
                    SameSite = SameSiteMode.Strict, // «ащита от CSRF
                    Expires = DateTimeOffset.Now.AddMinutes(30),
                    Path = "/" // ƒоступно дл€ всех путей
                });
                // Ќе сохран€ем токен в сессии - это избыточно при использовании кук
                // HttpContext.Session.SetString("JWTToken", responseContent.Token);

                // ѕеренаправл€ем на защищенную страницу
                return RedirectToPage("/UserPages");
            }
            
            ModelState.AddModelError(string.Empty, "Ќеверные учетные данные");
            return Page();
        }

        private class AuthResponse
        {
            public string Token { get; set; }
            public int UserId { get; set; }
            public string Username { get; set; }
            public string Role { get; set; }
        }
        
    }
}
