using GlobalVariablesRP;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Model.Order;
using OrderService.BLL.Dto.Order;
using System.Net.Http.Headers;
using System.Text.Json;

namespace WebApplication1.Pages
{
    [Authorize]
    public class OrderItemModel : PageModel
    {
        private readonly HttpClient _client;

        public OrderItemModel(IHttpClientFactory clientFactory)
        {
            _client = clientFactory.CreateClient();
            _client.BaseAddress = new Uri(GlobalVariables.GATEWAY);
        }

        [BindProperty(SupportsGet = true)]
        public Order Order { get; set; }

        public async Task OnGetAsync()
        {
            Order = new Order
            {
                Id = 1,
            };
        }

        public async Task<JsonResult> OnGetOrderUser()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["JWTToken"]);
            var responseUser = await _client.GetAsync(
               "https://localhost:7042/api/Order/GetAllOrderUser");

            var dsdsds = await _client.GetAsync(
                      $"https://localhost:7042/api/Order/1");



            if (responseUser.IsSuccessStatusCode)
            {
                string responseBody = await responseUser.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    PropertyNameCaseInsensitive = true
                };
                var itemsOrder = JsonSerializer.Deserialize<List<OrderDto>>(responseBody, options);

                if (itemsOrder == null || itemsOrder.Count == 0) return new JsonResult(Array.Empty<object>());

                var result = itemsOrder
                    .Select(item => new
                    {
                        Id = item.Id,
                        UserId = item.UserId,
                        Price = 0, //общая сумма
                        Count = 0, //количество позиций
                        DateCreated = item.DateCreated,
                        ArriveDate = item.ArriveDate,
                        ShippingMethod = item.ShippingMethod,
                        PaymentMethod = item.PaymentMethod,
                        ArriveAddress = item.ArriveAddress,
                        OrderStatus = item.OrderStatus,
                        DateOrderStatus = item.DateOrderStatus,

                    })
                    .ToList();

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
        /*[HttpPost]
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
                    $"{GlobalVariables.GETWAY_OCELOT}{GlobalVariables.DELETE_PRODUCT_CART}")
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
                    $"{GlobalVariables.GETWAY_OCELOT}" +
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
        public async Task<IActionResult> OnPostSaveProductAsync([FromBody] AddProductDto product)
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

                // 2. Валидация данных
                if (string.IsNullOrWhiteSpace(product.NameProduct) || product.Price <= 0)
                {
                    return BadRequest("Название и цена товара обязательны");
                }
                _client.DefaultRequestHeaders.Authorization =
                  new AuthenticationHeaderValue("Bearer", Request.Cookies["JWTToken"]);

                var response = await _client.PostAsJsonAsync(
                    $"{GlobalVariables.GETWAY_OCELOT}{GlobalVariables.POST_ADD_PRODUCT}",
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
        }*/
    }
}