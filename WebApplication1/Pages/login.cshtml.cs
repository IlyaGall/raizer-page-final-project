using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Model.User;

namespace WebApplication1.Pages
{
   // [Authorize]
    public class LoginModel : PageModel
    {
        private readonly JwtService _jwtService;
        public void OnGet()
        {
        }
        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public LoginModel(JwtService jwtService)
        {
            _jwtService = jwtService;
        }


       




        public IActionResult OnPost()
        {
            //TODO!!!
            // Здесь должна быть логика проверки учетных данных, тут у Глеба уточнить
            // В реальном приложении проверяем в бд в базе данных
            if (Username == "1" && Password == "1")
            {// тут наверное нужно делать запрос в бд имя+пароль и ждать ответ и записывать его в jwtService
                User user = new User();
                user.test();
                var token = _jwtService.GenerateToken(user, new[] { "Admin" });

                //TODO проверить с docker 
                // Сохраняем токен в куки
                Response.Cookies.Append("jwt", token, new CookieOptions
                {
                    HttpOnly = false, // вот эту фигню нужно сделать false иначе js не будет работать с cookie, РЕДИСКА!!!
                    // делает токен доступным для XSS-атак. помучать Глеба, как будем всё же делать, но для тестирования сойдёт?!

                    Secure = true, // Только для HTTPS
                    SameSite = SameSiteMode.Lax, // Или Strict, если все на одном домене, но как будет работать с docker я пока не знаю
                    Expires = DateTimeOffset.Now.AddHours(1)
                });

                return RedirectToPage("/Index");
                //после удачной авторизации идёт переадресация на сайт магазина
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return Page();
        }
    }
}
