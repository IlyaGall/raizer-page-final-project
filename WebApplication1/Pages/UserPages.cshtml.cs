using ConnectBackEnd;
using GlobalVariablesRP;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AuthService.Dto;
using System.Net.Http.Headers;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebApplication1.Pages
{
    [Authorize]
    public class UserPagesModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _client;

        public UserPagesModel(IConfiguration configuration, IHttpClientFactory clientFactory)
        {
            _configuration = configuration;
            _client = clientFactory.CreateClient();
            _client.BaseAddress = new Uri(GlobalVariables.GETWAY_OCELOT);
        }

        /// <summary>
        /// Выход из личного кабинета путём удаления cook
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnPostLogoutAsync()
        {
            try
            {
                // Удаляем JWT токен из куков
                Response.Cookies.Delete("JWTToken", new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Path = "/"
                });
            }
            catch
            {
                return RedirectToPage("/Index");

            }
            // Перенаправляем на страницу входа
            //  return LocalRedirect("/Index");
            return RedirectToPage("/Index");
        }


        /// <summary>
        /// Для тестирования jwt потом удалить
        /// </summary>
        /// <returns></returns>
        public IActionResult OnGet()
        {
            // Получаем JWT из куки
            var jwtToken = Request.Cookies["JWTToken"];

            if (jwtToken != null)
            {
                // Парсим JWT вручную
                var handler = new JwtSecurityTokenHandler();
                var token_user = handler.ReadJwtToken(jwtToken);

                // Получаем данные из payload
                var userId = token_user.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
                var username_token = token_user.Claims.First(c => c.Type == ClaimTypes.Name).Value;
                var role = token_user.Claims.First(c => c.Type == ClaimTypes.Role).Value;

                ViewData["UserId"] = userId;
                ViewData["Username"] = username_token;
                ViewData["Role"] = role;
                //  ViewData["Nickname"] = nickname;
            }

            var username = User.Identity?.Name;  // Получаем имя пользователя из JWT
            ViewData["Username"] = username;

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtSettings = _configuration.GetSection("JwtSettings");
                var secretKey = jwtSettings["SecretKey"];
            }
            catch (Exception ex) { }
            return Page();
        }


        [BindProperty]
        public AddUserDto UpdateData { get; set; }

        public async Task<IActionResult> OnPostUpdateUserInfoAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["JWTToken"]);
                var response = await _client.PostAsJsonAsync($"{GlobalVariables.GETWAY_OCELOT}/auth/update", UpdateData);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Данные успешно обновлены";
                    return RedirectToPage();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError(string.Empty, $"Ошибка при обновлении данных: {errorContent}");
                    return Page();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Произошла ошибка: {ex.Message}");
                return Page();
            }
        }


        #region подгрузка данных по базам

        // Метод для обработки AJAX запросов
        public async Task<JsonResult> OnGetUserData()
        {
            using var apiClient = new ConnectServer();
            var user = await apiClient.GetAsync<UserDto>(GlobalVariables.GET_INFO_USER, Request.Cookies["JWTToken"]);
            // Здесь можно получить данные пользователя из базы данных
            var userData = new
            {
                FullName = $"{user.Data.Surname} {user.Data.Name} {user.Data.Patronymic}",
                Email = $"{user.Data.Email}",
                Phone = $"{user.Data.NumberPhone}",
                Position = "Менеджер по продажам",
                Department = "Отдел продаж",
                HireDate = "15.03.2020"
            };

            return new JsonResult(userData);
        }

        public JsonResult OnGetFavorites()
        {
            // Здесь можно получить избранные товары из базы данных
            var favorites = new[]
            {
                new
                {
                    Id = 1,
                    Name = "Ноутбук Lenovo IdeaPad 5",
                    Price = "54 990 ₽",
                    Image = "https://via.placeholder.com/50",
                    Category = "Ноутбуки"
                },
                new
                {
                    Id = 2,
                    Name = "Смартфон Samsung Galaxy S21",
                    Price = "69 990 ₽",
                    Image = "https://via.placeholder.com/50",
                    Category = "Смартфоны"
                }
            };

            return new JsonResult(favorites);
        }

        public async Task<JsonResult> OnGetShops()
        {
            // Здесь можно получить магазины пользователя из базы данных
            try
            {

               // using var client = new HttpClient();
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["JWTToken"]);
                
                var response = await _client.GetAsync(
                    $"{GlobalVariables.GETWAY_OCELOT}" +
                    $"{GlobalVariables.POST_LIST_SHOP_USER}"); //Объект не нужен, так как передаём jwt токен


                if (!response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Ошибка при получении данных";
                    return null;
                }

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();


                    // Далее десериализация JSON 
                    var items = JsonSerializer.Deserialize<List<ObjectDto>>(responseBody);

                    if (items == null || items.Count == 0)
                    {
                        return null;
                    }
                    var result = items.Select(item => new
                    {
                        id = item.ShopId,
                        name = item.NameShop,
                        products = 0
                    })
                     .ToList();

                        
                  return new JsonResult(result);
                   
                }
                return new JsonResult(null);
            }
            catch (Exception ex)
            {
                TempData["SuccessMessage"] = ex.Message ;
                return new JsonResult(new { error = ex.Message });
            }
        }

        #endregion
    }

    public class ObjectDto
    {
        public int UserId { get; set; }
        public int ShopId { get; set; }
        public string NameShop { get; set; }
    }
}


