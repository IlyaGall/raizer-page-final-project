using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Model.Product;
using System.Security.AccessControl;
using ConnectBackEnd;
using WebApplication1.Model.Product.ProductDto;
using System.Xml.Linq;
using GlobalVariablesRP;
using AuthService.BLL.Dto;
using ManagersShopsService.BLL.Dto;
using System.Net.Http.Headers;
using System.Text.Json;
using CartService.BLL;
using ShopService.Domain;

namespace WebApplication1.Pages
{
    // https://localhost:7100/Product?name=ds
    public class ProductModel : PageModel
    {
        private readonly HttpClient _client;

        public ProductModel(IHttpClientFactory clientFactory)
        {
            _client = clientFactory.CreateClient();
            _client.BaseAddress = new Uri(GlobalVariables.GETWAY_OCELOT);
        }

        public string Message { get; private set; } = "";
        private string IdProduct { get; set; } = string.Empty;
        public CartDto Cart { get; set; } = new CartDto();

        public void OnGet(string id)
        {
            Message = $"Id: {id}";
            IdProduct = id;
        }

        /// <summary>
        /// Загрузка информации о товаре
        /// </summary>
        public void LoadInfoProduct()
        {

        }

        /// <summary>
        /// API endpoint для получения информации о товаре
        /// </summary>
        public async Task<IActionResult> OnGetProductInfoAsync(string id)
        {
            // Здесь вы получаете данные о товаре из базы данных или другого источника

            var productInfo = await LoadInfoProduct(id);

            return new JsonResult(productInfo);
        }

        /// <summary>
        /// Загрузка информации о товаре
        /// </summary>
        private async Task<object> LoadInfoProduct(string id)
        {
            using var apiClient = new ConnectServer();
            var products = await apiClient.GetAsync<ProductDto>(GlobalVariables.GET_PRODUCT_ID + id);
            return new
            {
                Id = products.Data.ProductId,
                IdShop = products.Data.ShopId,
                products.Data.ClusterId,
                NameProduct = products.Data.NameProduct,
                Description = products.Data.Description,
                Price = products.Data.Price,
                products.Data.Barcode,
                ModelNumber = products.Data.ModelNumber,
                ImageUrl = "/images/product.jpg",
                Amount = 12 
            };
        }

        /// <summary>
        /// Добавления продукта в корзину
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken] // Атрибут на серверной стороне для проверки токена
        public async Task<IActionResult> OnPostAddCartProduct([FromBody] AddCartDto request)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            };
            try
            {
                // 1. Проверка аутентификации
                if (!User.Identity.IsAuthenticated)
                {
                    return BadRequest(new { message = "Требуется авторизация" });
                }

                // 2. Базовая валидация
                //if (string.IsNullOrWhiteSpace(request.UserName))
                {
                    //return BadRequest(new { message = "Имя пользователя обязательно" });
                }

                if (request.UserId <= 0)
                {
                    return BadRequest(new { message = "Некорректный ID пользователя" });
                }

                // 3. Проверка существования пользователя
                var responseUser = await _client.GetAsync(
                    $"{GlobalVariables.GETWAY_OCELOT}{GlobalVariables.GET_USER_ID}{request.UserId}");

                if (!responseUser.IsSuccessStatusCode)
                {
                    return BadRequest(new { message = "Пользователь с указанным ID не найден" });
                }

                // 4. Проверка соответствия данных
                var responseBody = await responseUser.Content.ReadAsStringAsync();
                var item = JsonSerializer.Deserialize<UserEasyDto>(responseBody, options);

                //if (item.Id != request.UserId || item.Login != request.UserName)
                {
                    //return BadRequest(new { message = "ID или логин пользователя не совпадают. Пользователя не найдено!" });
                }

                // 5. Добавление менеджера
                _client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", Request.Cookies["JWTToken"]);

                AddCartDto addCartDto = new();
                addCartDto.ProductId = request.ProductId;
                addCartDto.Count = request.Count;
                addCartDto.Discount = request.Discount;
                addCartDto.UserId = request.UserId;
                addCartDto.Price = request.Price;
                var response = await _client.PostAsJsonAsync($"{GlobalVariables.GETWAY_OCELOT}{GlobalVariables.POST_ADD_CART}", addCartDto);

                if (response.IsSuccessStatusCode)
                {
                    return new JsonResult(new { success = true });
                }

                return new JsonResult(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Произошла ошибка: {ex.Message}" });
            }
        }
    }
}
