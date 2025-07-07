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
                        id = item.Id,
                        userId = item.UserId,
                        dateCreated = item.DateCreated.ToShortDateString(),
                        arriveDate = item.ArriveDate.ToShortDateString(),
                        shippingMethod = item.ShippingMethod == 0 ? "Самовывоз" : "Курьером",
                        paymentMethod = item.PaymentMethod == 0 ? "Кредитная карта" : "Наличные",
                        arriveAddress = item.ArriveAddress,
                        orderStatus = item.OrderStatus == 0 ? "Ожидает оплату" : (item.OrderStatus == 1 ? "В пути" : "Доставлен"),
                        dateOrderStatus = item.DateOrderStatus.ToShortDateString(),

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
    }
}