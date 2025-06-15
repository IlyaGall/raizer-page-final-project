using AuthService.Dto;
using ConnectBackEnd;
using GlobalVariablesRP;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;

namespace WebApplication1.Pages
{
   
        [Authorize]
        public class EditProfileModel : PageModel
        {
            private readonly IConfiguration _configuration;

            public EditProfileModel(IConfiguration configuration)
            {
                _configuration = configuration;
            }

            [BindProperty]
            public UpdateDateDto UpdateData { get; set; } = new UpdateDateDto();

            public async Task OnGetAsync()
            {
                var jwtToken = Request.Cookies["JWTToken"];

                if (!string.IsNullOrEmpty(jwtToken))
                {
                    using var apiClient = new ConnectServer();
                    var user = await apiClient.GetAsync<UserDto>(GlobalVariables.GET_INFO_USER, jwtToken);

                    if (user != null && user.Data != null)
                    {
                        UpdateData = new UpdateDateDto
                        {
                            Name = user.Data.Name,
                            Surname = user.Data.Surname,
                            Patronymic = user.Data.Patronymic,
                            Login = user.Data.Login,
                            Email = user.Data.Email,
                            NumberPhone = user.Data.NumberPhone,
                            TelegramID = user.Data.TelegramID
                        };
                    }
                }
            }

            public async Task<IActionResult> OnPostAsync()
            {
                if (!ModelState.IsValid)
                {
                    return Page();
                }

                try
                {
                    using var client = new HttpClient();
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["JWTToken"]);

                    var response = await client.PostAsJsonAsync($"{GlobalVariables.GETWAY_OCELOT}/gateway/auth/update", UpdateData);

                    if (response.IsSuccessStatusCode)
                    {
                        TempData["SuccessMessage"] = "Данные успешно обновлены";
                        return RedirectToPage("/UserPages");
                    }

                    var errorContent = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError(string.Empty, $"Ошибка при обновлении данных: {errorContent}");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"Произошла ошибка: {ex.Message}");
                }

                return Page();
            }
        }
}
