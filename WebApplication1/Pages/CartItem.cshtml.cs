using CartService.BLL;
using ConnectBackEnd;
using GlobalVariablesRP;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Model.Cart;
using OrderService.Order;
using ProductService.BLL;
using ShopService.Domain;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using WebApplication1.Model.Product.ProductDto;

namespace WebApplication1.Pages
{
    [Authorize]
    public class CartItemModel : PageModel
    {
        private readonly HttpClient _client;

        public CartItemModel(IHttpClientFactory clientFactory)
        {
            _client = clientFactory.CreateClient();
            _client.BaseAddress = new Uri(GlobalVariables.GATEWAY);
        }

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }
        public Cart Cart { get; set; }

        public async Task OnGetAsync()
        {
            var response = await _client.GetAsync(
                _client.BaseAddress +
                $"{GlobalVariables.GET_INFO_SHOP}{Id}");

            if (response.IsSuccessStatusCode)
            {

                string responseBody = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    PropertyNameCaseInsensitive = true
                };
                var item = JsonSerializer.Deserialize<Shop>(responseBody, options);

                Cart = new Cart
                {
                    Id = item.Id,
                    Name = item.Name,
                    Description = item.Description,
                };
            }
            else
            {
                Cart = new Cart
                {
                    Id = 1,
                    Name = "1",
                    Description = "1",
                };
            }
        }

        public async Task<JsonResult> OnGetCartUser()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["JWTToken"]);
            var responseUser = await _client.GetAsync(
            _client.BaseAddress +
            $"{GlobalVariables.GET_INFO_CART}");
            //"https://localhost:7175/api/Cart/GetCartUser");

            if (responseUser.IsSuccessStatusCode)
            {
                string responseBody = await responseUser.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    PropertyNameCaseInsensitive = true
                };
                var itemsCart = JsonSerializer.Deserialize<List<GetCartListDto>>(responseBody, options);

                if (itemsCart == null || itemsCart.Count == 0) return new JsonResult(Array.Empty<object>());

                //Получаем продукты
                var result = new List<Cart>();

                foreach (var cart in itemsCart)
                {
                    using var apiClient = new ConnectServer();
                    var item = await apiClient.GetAsync<ProductDto>(
                        _client.BaseAddress +
                        $"{GlobalVariables.GET_PRODUCT_ID}{cart.ProductId}");
                        //$"https://localhost:7186/api/Product/GetCartUser?id={cart.ProductId}");

                    if (item == null) continue;

                    result.Add(
                        new Cart
                        {
                            Id = cart.Id,
                            Price = item.Data.Price,
                            Name = item.Data.NameProduct,
                            Description = item.Data.Description,
                            ModelNumber = item.Data.ModelNumber,
                            Count = cart.Count,
                        });
                }

                return new JsonResult(result);
            }
            else
            {
                TempData["SuccessMessage"] += "Ошибка при получении данных из корзины";
            }

            return new JsonResult(Array.Empty<object>());
        }

        /// <summary>
        /// Удаление товара из корзины
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostDeleteProduct([FromBody] DeleteProductDto deleteDto)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["JWTToken"]);

                var request = new HttpRequestMessage(
                    HttpMethod.Delete,
                    _client.BaseAddress +
                    $"{GlobalVariables.DELETE_PRODUCT_CART}")
                {
                    Content = new StringContent(
                    JsonSerializer.Serialize(deleteDto),
                    Encoding.UTF8,
                    "application/json")
                };

                var response = await _client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    return new OkResult();
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return BadRequest();
        }

        /// <summary>
        /// Удаление товара из корзины
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostDeleteCart([FromBody] DeleteProductDto deleteDto)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["JWTToken"]);

                var request = new HttpRequestMessage(
                    HttpMethod.Delete,
                    _client.BaseAddress +
                    $"{GlobalVariables.DELETE_ALL_PRODUCT_CART}")
                {
                    Content = new StringContent(
                    JsonSerializer.Serialize(deleteDto),
                    Encoding.UTF8,
                    "application/json")
                };

                var response = await _client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    return new OkResult();
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return BadRequest();
        }

        /// <summary>
        /// Обновление количества 
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostUpdateProduct([FromBody] UpdateProductDto request)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowNamedFloatingPointLiterals
            };

            if (!ModelState.IsValid)
            {
                return Page();
            }
            try
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["JWTToken"]);
                var response = await _client.PutAsJsonAsync(
                    _client.BaseAddress +
                    $"{GlobalVariables.PUT_CART_UPDATE}",
                    request);

                if (!response.IsSuccessStatusCode)
                {
                    return BadRequest(await response.Content.ReadAsStringAsync());
                }
                return new JsonResult(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Добовление продукта
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken] // Атрибут на серверной стороне для проверки токена
        public async Task<IActionResult> OnPostCreateOrder([FromBody] AddOrderDto product)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowNamedFloatingPointLiterals
            };

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // 1. Проверка авторизации
                if (!User.Identity.IsAuthenticated)
                {
                    return Unauthorized();
                }

                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["JWTToken"]);
                var response = await _client.PostAsJsonAsync(
                        _client.BaseAddress +
                        $"{GlobalVariables.POST_ADD_ORDER}",
                        //"https://localhost:7042/api/Order/AddOrder",
                    product,
                    options);

                if (!response.IsSuccessStatusCode)
                {
                    return BadRequest(await response.Content.ReadAsStringAsync());
                }

                // 5. Возвращаем успешный результат
                return new JsonResult(new { success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при сохранении товара: {ex.Message}");
            }
        }
    }
}