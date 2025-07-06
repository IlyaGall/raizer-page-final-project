using GlobalVariablesRP;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Model.Order;
using OrderService.BLL.Dto.Order;
using System.Net.Http.Headers;

using System.Text;
using System.Text.Json;
using WebApplication1.Model.Product.ProductDto;


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
                $"{GlobalVariables.GETWAY_OCELOT}" +
                $"{GlobalVariables.GET_INFO_ALL_ORDER}");


            if (responseUser.IsSuccessStatusCode)
            {
                string responseBody = await responseUser.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    PropertyNameCaseInsensitive = true
                };

                var item = JsonSerializer.Deserialize<Shop>(responseBody, options);

                Shop = new Shop
                {
                    Id = item.Id,
                    Name = item.Name,
                    Description = item.Description,
                    Adress = item.Adress,
                    ContactInfo = item.ContactInfo
                };
                //TempData["SuccessMessage"] = "Данные успешно обновлены";
            }
            else
            {

                TempData["SuccessMessage"] += "Ошибка при обновлении данных по магазину";

            }

        }


        #region Работа с магазином
        [HttpPost]
        [ValidateAntiForgeryToken] // Атрибут на серверной стороне для проверки токена
        public async Task<IActionResult> OnPostUpdateShop([FromBody] UpdateShopDto request)
        {
            // Токен уже проверен автоматически
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

                _client.DefaultRequestHeaders.Authorization =
              new AuthenticationHeaderValue("Bearer", Request.Cookies["JWTToken"]);

                var response = await _client.PutAsJsonAsync(
                    $"{GlobalVariables.GATEWAY}{GlobalVariables.PUT_SHOP_UPDATE}",
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


        #endregion



        #region  Работа с товарами

        /// <summary>
        /// Загрузка товаров для магазина из бд
        /// </summary>
        /// <param name="shopId"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken] // Атрибут на серверной стороне для проверки токена
        public async Task<JsonResult> OnGetProducts(int shopId)
        {


            // Получаем товары магазина из БД
            // var products = await _productService.GetProductsByShopAsync(shopId);
            // Токен уже проверен автоматически
            try
            {
                var response = await _client.GetAsync(
                  $"{GlobalVariables.GATEWAY}" +
                  $"{GlobalVariables.GET_SHOP_PRODUCTS}{shopId}");
                string s = $"{GlobalVariables.GATEWAY}" +
                  $"{GlobalVariables.GET_SHOP_PRODUCTS}{shopId}";
                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();

                    var options = new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        PropertyNameCaseInsensitive = true
                    };
                    var items = JsonSerializer.Deserialize<List<ProductDto>>(responseBody, options);

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



                var request = new HttpRequestMessage(HttpMethod.Delete,
            $"{GlobalVariables.GATEWAY}{GlobalVariables.DELETE_PRODUCT}")

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
                    $"{GlobalVariables.GATEWAY}{GlobalVariables.POST_ADD_PRODUCT}",
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
        #endregion


        #region менеджеры
        /// <summary>
        /// загрузка менеджеров магазина
        /// </summary>
        /// <param name="shopId"></param>
        /// <returns></returns>
        public async Task<JsonResult> OnGetManagers(int shopId)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            };
            // Получаем менеджеров магазина из БД
            // var managers = await _shopService.GetShopManagersAsync(shopId);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["JWTToken"]);

            var responseUser = await _client.GetAsync(
                  $"{GlobalVariables.GATEWAY}{GlobalVariables.GET_MANAGER_SHOP}{shopId}");
            if (!responseUser.IsSuccessStatusCode)
            {
                return new JsonResult("");
            }
            var responseBody = await responseUser.Content.ReadAsStringAsync();
            var items = JsonSerializer.Deserialize<List<GetManagersShopsDto>>(responseBody, options);

            var result = items.Select(item => new
            {
                Id = item.Id,
                Name = item.UserName,
                Email = item.RoleUser
            })
                   .ToList();
            return new JsonResult(result);
        }

        /// <summary>
        /// Удаление менеджера
        /// </summary>
        /// <param name="managerId"></param>
        /// <param name="shopId"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnPostRemoveManagerAsync([FromBody] DeleteManagersShopsDto deleteDto)
        {
            try
            {

                _client.DefaultRequestHeaders.Authorization =
          new AuthenticationHeaderValue("Bearer", Request.Cookies["JWTToken"]);


                var request = new HttpRequestMessage(HttpMethod.Delete,
            $"{GlobalVariables.GATEWAY}{GlobalVariables.DELETE_MANAGER_SHOP}")
                {
                    Content = new StringContent(
                JsonSerializer.Serialize(deleteDto),
                Encoding.UTF8,
                "application/json")
                };

                var response = await _client.SendAsync(request);

                // Отправляем DELETE запрос с телом

                if (response.IsSuccessStatusCode)
                {
                    return new OkResult();
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Добавления нового менеджера
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken] // Атрибут на серверной стороне для проверки токена
        public async Task<IActionResult> OnPostAddManagerAsync([FromBody] AddManagersShopsDto request)
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
                if (string.IsNullOrWhiteSpace(request.UserName))
                {
                    return BadRequest(new { message = "Имя пользователя обязательно" });
                }

                if (request.UserId <= 0)
                {
                    return BadRequest(new { message = "Некорректный ID пользователя" });
                }

                // 3. Проверка существования пользователя
                var responseUser = await _client.GetAsync(
                    $"{GlobalVariables.GATEWAY}{GlobalVariables.GET_USER_ID}{request.UserId}");

                if (!responseUser.IsSuccessStatusCode)
                {
                    return BadRequest(new { message = "Пользователь с указанным ID не найден" });
                }

                // 4. Проверка соответствия данных
                var responseBody = await responseUser.Content.ReadAsStringAsync();
                var item = JsonSerializer.Deserialize<UserEasyDto>(responseBody, options);

                if (item.Id != request.UserId || item.Login != request.UserName)
                {
                    return BadRequest(new { message = "ID или логин пользователя не совпадают. Пользователя не найдено!" });
                }

                // 5. Добавление менеджера
                _client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", Request.Cookies["JWTToken"]);

                //var response = await _client.PostAsJsonAsync(
                //    $"{GlobalVariables.GETWAY_OCELOT}{GlobalVariables.POST_ADD_MANAGER_SHOP}",
                //    request);

                //if (!response.IsSuccessStatusCode)
                //{
                //    var errorContent = await response.Content.ReadAsStringAsync();
                //    return BadRequest(new { message = $"Ошибка при добавлении: {errorContent}" });
                //}

                AddManagersShopsDto addManagersShopsDto = new();
                addManagersShopsDto.NameShop = request.NameShop;
                addManagersShopsDto.ShopId = request.ShopId;
                addManagersShopsDto.UserId = request.UserId;
                addManagersShopsDto.RoleUser = request.RoleUser;
                addManagersShopsDto.UserName = request.UserName;
                var response = await _client.PostAsJsonAsync($"{GlobalVariables.GATEWAY}{GlobalVariables.POST_ADD_MANAGER_SHOP}", addManagersShopsDto);



                if (response.IsSuccessStatusCode)
                {
                    // return RedirectToPage("/UserPages");
                    return new JsonResult(new { success = true });
                }

                return new JsonResult(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Произошла ошибка: {ex.Message}" });
            }
        }
        #endregion

    }
}