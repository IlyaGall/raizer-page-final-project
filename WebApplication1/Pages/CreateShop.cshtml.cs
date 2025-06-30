using GlobalVariablesRP;
using ManagersShopsService.BLL.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;
using System.Security.Claims;
using WebApplication1.Model.Shop;

namespace WebApplication1.Pages
{
    [Authorize]
    public class CreateShopModel() : PageModel
    {
        //private readonly IHttpClientFactory _httpClientFactory; // фабрика клиентов
        //private readonly IConfiguration _configuration;

        //public CreateShopModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        //{
        //    _httpClientFactory = httpClientFactory;
        //    _configuration = configuration;
        //}




        [BindProperty]
        public ShopInputModel Input { get; set; }

        public class ShopInputModel
        {
            [Required, MaxLength(60)]
            public string Name { get; set; }

            [Required]
            public string Description { get; set; }

            [Required]
            public string ContactInfo { get; set; }

            [Required]
            public string Address { get; set; }
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                // Получаем ID текущего пользователя
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                // Создаем объект магазина
                var shop = new AddShop
                {
                    Name = Input.Name,
                    Description = Input.Description,
                    ContactInfo = Input.ContactInfo,
                    Adress = Input.Address,
                };

                AddManagersShopsDto addManagersShopsDto = new()
                {
                    RoleUser = "Administrator",
                };


                using var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["JWTToken"]);
                var response = await client.PostAsJsonAsync($"{GlobalVariables.GATEWAY}{GlobalVariables.POST_ADD_SHOP}", shop);

                if (response.IsSuccessStatusCode)
                {
                    addManagersShopsDto.ShopId = Convert.ToUInt16(await response.Content.ReadAsStringAsync());
                    addManagersShopsDto.NameShop = shop.Name;

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["JWTToken"]);
                    response = await client.PostAsJsonAsync($"{GlobalVariables.GATEWAY}{GlobalVariables.POST_ADD_OWNER_SHOP}", addManagersShopsDto);

                    if (response.IsSuccessStatusCode)
                    {
                        // return RedirectToPage("/UserPages");
                        return RedirectToPage("/ShopManagement", new { id = addManagersShopsDto.ShopId });
                    }
                }
                else
                {
                    TempData["SuccessMessage"] = await response.Content.ReadAsStringAsync();
                }
                return null;

            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Ошибка при создании магазина: {ex.Message}");
                return Page();
            }
        }

    }
}
