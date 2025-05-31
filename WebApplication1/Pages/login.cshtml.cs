using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

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
            if (Username == "admin" && Password == "password")
            {
                var token = _jwtService.GenerateToken(Username, new[] { "Admin" });

                // Сохраняем токен в куки
                Response.Cookies.Append("jwt", token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict
                });

                return RedirectToPage("/Index");
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return Page();
        }
    }
}
