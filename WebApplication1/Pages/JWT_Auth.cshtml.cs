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
        private readonly IHttpClientFactory _httpClientFactory; // ������� ��������
        private readonly IConfiguration _configuration;

        public JWT_AuthModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        // ���� ������ � ����� �����������
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

                // �������������� �� ���������� ��������
                return RedirectToPage("/UserPages");
            }
            
            ModelState.AddModelError(string.Empty, "�������� ������� ������");
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
                TempData["SuccessMessage"] = "����������� ������ �������! ������ �� ������ ����� � �������.";
                ActiveTab = "login";
                return RedirectToPage(new { tab = "login" });
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, $"������ �����������: {errorContent}");
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
            /// ��� ������������
            /// </summary>
            [Required(ErrorMessage = "��� �����������")]
            [StringLength(50, ErrorMessage = "��� �� ������ ��������� 50 ��������")]
            public string Name { get; set; } = string.Empty;

            /// <summary>
            /// ������� ������������
            /// </summary>
            [Required(ErrorMessage = "������� �����������")]
            [StringLength(50, ErrorMessage = "������� �� ������ ��������� 50 ��������")]
            public string Surname { get; set; } = string.Empty;

            /// <summary>
            /// ��������
            /// </summary>
            [StringLength(50, ErrorMessage = "�������� �� ������ ��������� 50 ��������")]
            public string Patronymic { get; set; } = string.Empty;

            /// <summary>
            /// �����
            /// </summary>
            [Required(ErrorMessage = "����� ����������")]
            [StringLength(20, MinimumLength = 4, ErrorMessage = "����� ������ ���� �� 4 �� 20 ��������")]
            public string Login { get; set; } = string.Empty;

            /// <summary>
            /// ������ ������������
            /// </summary>
            [Required(ErrorMessage = "������ ����������")]
            [StringLength(100, MinimumLength = 6, ErrorMessage = "������ ������ ���� �� 6 �� 100 ��������")]
            [DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;

            /// <summary>
            /// ��������� ������� ������������
            /// </summary>
            [Phone(ErrorMessage = "�������� ������ ��������")]
            public string NumberPhone { get; set; } = string.Empty;

            /// <summary>
            /// ����� ������������
            /// </summary>
            [EmailAddress(ErrorMessage = "�������� ������ email")]
            public string Email { get; set; } = string.Empty;

            /// <summary>
            /// ����������� ������������
            /// </summary>
            [Range(1, long.MaxValue, ErrorMessage = "Telegram ID ������ ���� ������������� ������")]
            public long TelegramID { get; set; }
        }
    }
}
