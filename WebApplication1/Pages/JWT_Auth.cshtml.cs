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
        private readonly IHttpClientFactory _httpClientFactory; // ������� ��������
        private readonly IConfiguration _configuration;

        public JWT_AuthModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        // ���� ������ � ����� �����������

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
                //���� ������������ �����������, �� ��������� ��� ����� �� ��� ������ �������
                return RedirectToPage("/UserPages");
            }
            return null;
        }
        /// <summary>
        /// ��������� ����������� ������������
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnPostAsync()
        {
            var client = _httpClientFactory.CreateClient();

            client.BaseAddress = new Uri(_configuration["ApiBaseUrl"]);

            var loginData = new
            {
                // ���� ������ � �����
                LoginInput.Login,
                LoginInput.Password
            };

           
            var response = await client.PostAsJsonAsync("gateway/auth/login", loginData);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadFromJsonAsync<AuthResponse>();

             
                // ��������� ����� � ���� � ������
                Response.Cookies.Append("JWTToken", responseContent.Token, new CookieOptions
                {
                    HttpOnly = true, // ������ �� XSS js �� ������ �������� � ���������(
                    Secure = true, // ������ HTTPS
                    SameSite = SameSiteMode.Strict, // ������ �� CSRF
                    Expires = DateTimeOffset.Now.AddMinutes(30),
                    Path = "/" // �������� ��� ���� �����
                });
                // �� ��������� ����� � ������ - ��� ��������� ��� ������������� ���
                // HttpContext.Session.SetString("JWTToken", responseContent.Token);

                // �������������� �� ���������� �������� (������ �������� ������������)
                return RedirectToPage("/UserPages");
            }
            
            ModelState.AddModelError(string.Empty, "�������� ������� ������");
            ActiveTab = "login";
            return Page();
        }

        /// <summary>
        /// ����������� ������ ������������
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
                TempData["SuccessMessage"] = "����������� ������ �������! ������ �� ������ ����� � �������.";
                ActiveTab = "login";
                return RedirectToPage(new { tab = "login" });
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, $"������ �����������: {errorContent}");
            ActiveTab = "register";
            return Page();
        }
     
    }
}
