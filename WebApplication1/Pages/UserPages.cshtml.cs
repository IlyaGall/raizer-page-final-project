using ConnectBackEnd;
using GlobalVariablesRP;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using WebApplication1.Model.Auth;
using WebApplication1.Model.Auth.AuthService.BLL.Dto;
using WebApplication1.Model.Product.ProductDto;
using Microsoft.JSInterop;

namespace WebApplication1.Pages
{
    [Authorize]
    public class UserPagesModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public UserPagesModel(IConfiguration configuration)
        {
            _configuration = configuration;
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
            return LocalRedirect("/Index");
        }

        /// <summary>
        /// Выход из личного кабинета
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

        /// <summary>
        /// Получить данные пользователя из бд
        /// </summary>
        /// <returns></returns>
        private async Task<ApiResponse<UserDto>>  GetUserId() 
        {
            using var apiClient = new ConnectServer();
            return await apiClient.GetAsync<UserDto>(GlobalVariables.GET_INFO_USER, Request.Cookies["JWTToken"]);
        }

        // Метод для обработки AJAX запросов
        public async Task<JsonResult> OnGetUserData()
        {
        var s =  await  GetUserId();
            // Здесь можно получить данные пользователя из базы данных
            var userData = new
            {
                FullName = "Иванов Иван Иванович1",
                Email = "ivanov@example.com",
                Phone = "+7 (999) 123-45-67",
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

        public JsonResult OnGetShops()
        {
            // Здесь можно получить магазины пользователя из базы данных
            var shops = new[]
            {
                new { Id = 1, Name = "Центральный магазин", Products = 245 },
                new { Id = 2, Name = "Северный филиал", Products = 189 }
            };

            return new JsonResult(shops);
        }
    }
}