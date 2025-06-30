using AuthService.Dto;
using GlobalVariablesRP;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

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
            _client.BaseAddress = new Uri(GlobalVariables.GATEWAY);
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
                var response = await _client.PostAsJsonAsync($"{GlobalVariables.GATEWAY}/auth/update", UpdateData);

                if (response.IsSuccessStatusCode)
                {
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
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            };
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["JWTToken"]);

            var response = await _client.GetAsync(
                $"{GlobalVariables.GATEWAY}" +
                $"{GlobalVariables.GET_INFO_USER}"); //Объект не нужен, так как передаём jwt токен

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();


                // Далее десериализация JSON 
                var items = JsonSerializer.Deserialize<UserDto>(responseBody, options);

                //var result = items.Select(item => new
                //{
                //    Login = item.Login,
                //    name = item.Name,
                //    surname = item.Surname,
                //    Patronymic = item.Patronymic,
                //    numberPhone = item.NumberPhone,
                //    Email = item.Email,
                //    telegramID = item.TelegramID,
                //})
                // .ToList();




                return new JsonResult(items);
            }
            return new JsonResult("");
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
                    $"{GlobalVariables.GATEWAY}" +
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
                TempData["SuccessMessage"] = ex.Message;
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


