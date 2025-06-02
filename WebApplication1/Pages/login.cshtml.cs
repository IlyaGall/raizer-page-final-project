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
            // ����� ������ ���� ������ �������� ������� ������, ��� � ����� ��������
            // � �������� ���������� ��������� � �� � ���� ������
            if (Username == "1" && Password == "1")
            {// ��� �������� ����� ������ ������ � �� ���+������ � ����� ����� � ���������� ��� � jwtService
                User user = new User();
                user.test();
                var token = _jwtService.GenerateToken(user, new[] { "Admin" });

                //TODO ��������� � docker 
                // ��������� ����� � ����
                Response.Cookies.Append("jwt", token, new CookieOptions
                {
                    HttpOnly = false, // ��� ��� ����� ����� ������� false ����� js �� ����� �������� � cookie, �������!!!
                    // ������ ����� ��������� ��� XSS-����. �������� �����, ��� ����� �� �� ������, �� ��� ������������ �����?!

                    Secure = true, // ������ ��� HTTPS
                    SameSite = SameSiteMode.Lax, // ��� Strict, ���� ��� �� ����� ������, �� ��� ����� �������� � docker � ���� �� ����
                    Expires = DateTimeOffset.Now.AddHours(1)
                });

                return RedirectToPage("/Index");
                //����� ������� ����������� ��� ������������� �� ���� ��������
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return Page();
        }
    }
}
