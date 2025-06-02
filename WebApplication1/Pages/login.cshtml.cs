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
            // ����� ������ ���� ������ �������� ������� ������, ��� � ����� ��������
            // � �������� ���������� ��������� � �� � ���� ������
            if (Username == "1" && Password == "1")
            {
                var token = _jwtService.GenerateToken(Username, new[] { "Admin" });

                // ��������� ����� � ����
                Response.Cookies.Append("jwt", token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true, // ������ HTTPS
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTimeOffset.Now.AddHours(1),
                    Domain = "localhost",
                    Path = "/"
                });

                 return RedirectToPage("/Index");
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return Page();
        }
    }
}
