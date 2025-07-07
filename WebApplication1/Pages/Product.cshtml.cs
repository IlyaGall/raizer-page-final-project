using AuthService.BLL.Dto;
using CartService.BLL;
using CommentService.BLL;
using ConnectBackEnd;
using GlobalVariablesRP;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Model.Product;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using WebApplication1.Model.Comment;
using WebApplication1.Model.Product.ProductDto;

namespace WebApplication1.Pages
{
    // https://localhost:7100/Product?name=ds
    public class ProductModel : PageModel
    {
        public string Message { get; private set; } = "";
        public int IdProduct { get; set; } = 0;
        public CartDto Cart { get; set; } = new CartDto();

        public Product Product { get; set; }
        public int ShopId { get; set; }
        private readonly ILogger _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly HttpClient _client;
        private readonly string _jwtCookieName = "JWTToken";
        private readonly string _authHeaderValue = "Bearer";

        /// <summary>
        /// Êîíñòðóêòîð ñòðàíèöû
        /// </summary>
        /// <param name="clientFactory"></param>
        /// <param name="logger"></param>
        /// <exception cref="ArgumentNullException"/>
        public ProductModel(IHttpClientFactory clientFactory, ILogger<ProductModel> logger)
        {
            ArgumentNullException.ThrowIfNull(clientFactory);
            ArgumentNullException.ThrowIfNull(logger);
            _logger = logger;
            _httpClientFactory = clientFactory;
            _client = _httpClientFactory.CreateClient();
            _client.BaseAddress = new Uri(GlobalVariables.GATEWAY);
            _logger.LogDebug("New instance of ProductModel class was initialized");
        }
        public async Task OnGet(int id)
        {
            Message = $"Id: {id}";
            IdProduct = id;
            using var apiClient = new ConnectServer();
            var product = await apiClient.GetAsync<ProductDto>(GlobalVariables.GET_PRODUCT_ID + id);
            
            if (product.IsSuccess)
            {
                Product = new Product
                {
                    Id = product.Data.ProductId,
                    Name = product.Data.NameProduct,
                    Description = product.Data.Description,
                    Price = product.Data.Price,
                };
                ShopId = product.Data.ShopId;
            }
        }

        /// <summary>
        /// Çàãðóçêà èíôîðìàöèè î òîâàðå
        /// </summary>
        public void LoadInfoProduct()
        {
            _logger.LogDebug("Loading product info");
        }

        /// <summary>
        /// API endpoint äëÿ ïîëó÷åíèÿ èíôîðìàöèè î òîâàðå
        /// </summary>
        public async Task<IActionResult> OnGetProductInfoAsync(string id)
        {
            // Çäåñü âû ïîëó÷àåòå äàííûå î òîâàðå èç áàçû äàííûõ èëè äðóãîãî èñòî÷íèêà

            var productInfo = await LoadInfoProduct(id);

            return new JsonResult(productInfo);
        }

        /// <summary>

        /// Çàãðóçêà èíôîðìàöèè î òîâàðå
        /// </summary>
        private async Task<object> LoadInfoProduct(string id)
        {

            var response = await _client.GetAsync(
      $"{GlobalVariables.GATEWAY}{GlobalVariables.GET_PRODUCT_ID + id}");

            if (!response.IsSuccessStatusCode)
            {
                return BadRequest(new { message = "Òîâàð ñ óêàçàííûì ID íå íàéäåí" });
            }

            var responseBody = await response.Content.ReadAsStringAsync();

            // Äåñåðèàëèçóåì JSON ñ ó÷åòîì ñòðóêòóðû ApiResponse
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var apiResponse = JsonSerializer.Deserialize<ApiResponse<ProductDto>>(responseBody, options);

            if (!apiResponse.IsSuccess || apiResponse.Data == null)
            {
                return BadRequest(new { message = apiResponse.ErrorMessage ?? "Íå óäàëîñü ïîëó÷èòü äàííûå î òîâàðå" });
            }

            var product = apiResponse.Data;

            return new
            {
                Id = product.ProductId,
                IdShop = product.ShopId,
                product.ClusterId,
                NameProduct = product.NameProduct,
                Description = product.Description,
                Price = product.Price,
                product.Barcode,
                ModelNumber = product.ModelNumber,
                ImageUrl = "/images/product.jpg",
                Amount = 12
            };
        }

        /// <summary>
        /// Äîáàâëåíèÿ ïðîäóêòà â êîðçèíó
        /// </summary>

        [HttpPost]
        public async Task<IActionResult> OnPostProducts()
        {
            var responseUser = await _client.GetAsync(
                   $"{GlobalVariables.GATEWAY}{GlobalVariables.GET_USER_ID}{1}");
            return null;
        }


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
                // 1. Ïðîâåðêà àóòåíòèôèêàöèè
                if (!User.Identity.IsAuthenticated)
                {
                    return BadRequest(new { message = "Òðåáóåòñÿ àâòîðèçàöèÿ" });
                }

                // 2. Áàçîâàÿ âàëèäàöèÿ
                //if (string.IsNullOrWhiteSpace(request.UserName))
                {
                    //return BadRequest(new { message = "Èìÿ ïîëüçîâàòåëÿ îáÿçàòåëüíî" });
                }

                if (request.UserId <= 0)
                {
                    return BadRequest(new { message = "Íåêîððåêòíûé ID ïîëüçîâàòåëÿ" });
                }

                // 3. Ïðîâåðêà ñóùåñòâîâàíèÿ ïîëüçîâàòåëÿ
                var jwtToken = Request.Cookies["JWTToken"];

                if (jwtToken != null)
                {
                    // Парсим JWT вручную
                    var handler = new JwtSecurityTokenHandler();
                    var token_user = handler.ReadJwtToken(jwtToken);

                    // Получаем данные из payload
                    var userId = token_user.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
                   

                    request.UserId = Convert.ToInt32(userId);
                  
                    //  ViewData["Nickname"] = nickname;
                }



             
                var responseUser = await _client.GetAsync(
                    $"{GlobalVariables.GATEWAY}{GlobalVariables.GET_USER_ID}{request.UserId}");

                if (!responseUser.IsSuccessStatusCode)
                {
                    return BadRequest(new { message = "Ïîëüçîâàòåëü ñ óêàçàííûì ID íå íàéäåí" });
                }

                // 4. Ïðîâåðêà ñîîòâåòñòâèÿ äàííûõ
                var responseBody = await responseUser.Content.ReadAsStringAsync();
                var item = JsonSerializer.Deserialize<UserEasyDto>(responseBody, options);

                //if (item.Id != request.UserId || item.Login != request.UserName)
                {
                    //return BadRequest(new { message = "ID èëè ëîãèí ïîëüçîâàòåëÿ íå ñîâïàäàþò. Ïîëüçîâàòåëÿ íå íàéäåíî!" });
                }

                // 5. Äîáàâëåíèå ìåíåäæåðà
                _client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", Request.Cookies["JWTToken"]);

                AddCartDto addCartDto = new();
                addCartDto.ProductId = request.ProductId;
                addCartDto.Count = request.Count;
                addCartDto.Discount = request.Discount;
                addCartDto.UserId = request.UserId;
                addCartDto.Price = request.Price;
                
                var response = await _client.PostAsJsonAsync($"{GlobalVariables.GATEWAY}{GlobalVariables.POST_ADD_CART}", addCartDto);

                if (response.IsSuccessStatusCode)
                {
                    return new JsonResult(new { success = true });
                }

                return new JsonResult(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Ïðîèçîøëà îøèáêà: {ex.Message}" });
            }
        }







        public async Task<JsonResult> OnGetComments(string productId)
        {

            _client.DefaultRequestHeaders.Authorization =
                   new AuthenticationHeaderValue("Bearer", Request.Cookies["JWTToken"]);


            //_logger.LogDebug("Trying to load comments for product: " + productId);
            //using var client = _httpClientFactory.CreateClient();
            var response = await _client.GetAsync($"{GlobalVariables.GATEWAY}{GlobalVariables.GET_COMMENTS_BY_PRODUCT}{productId}");
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogInformation($"Failed to load comments: ");
                return new JsonResult(new { error = "Error" });
            }

            var content = await response.Content.ReadAsStringAsync();
            _logger.LogDebug("Comments response:" + content);
            var comments = JsonSerializer.Deserialize<CommentDto[]>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            });
            return new JsonResult(comments);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> OnPostAddComment([FromBody] AddCommentDto dto)
        {
            try
            {
                _logger.LogDebug("Trying to add comment");
                using var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue(_authHeaderValue, Request.Cookies[_jwtCookieName]);
                var request = new HttpRequestMessage(HttpMethod.Post,
                    $"{GlobalVariables.GATEWAY}{GlobalVariables.ADD_COMMENT}")
                {
                    Content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json")
                };

                var response = await client.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Api rejected add comment request");
                    return new JsonResult(new { success = false, error = await response.Content.ReadAsStringAsync() });
                }
                _logger.LogInformation("New comment created successfuly");
                return new JsonResult(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred on adding comment");
                return new JsonResult(new { success = false, error = ex.Message });
            }
        }
    }
}
