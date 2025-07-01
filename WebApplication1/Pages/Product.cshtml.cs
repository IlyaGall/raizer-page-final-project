using CommentService.BLL;
using ConnectBackEnd;
using GlobalVariablesRP;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Model.Product;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using WebApplication1.Model.Comment;
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
        public CartDto Cart { get; set; }

        public Product Product { get; set; }
        public int ShopId { get; set; }
        private readonly ILogger _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly HttpClient _client;
        private readonly string _jwtCookieName = "JWTToken";
        private readonly string _authHeaderValue = "Bearer";
        /// <summary>
        /// Конструктор страницы
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
        /// Загрузка информации о товаре
        /// </summary>
        public void LoadInfoProduct()
        {
            _logger.LogDebug("Loading product info");
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

        public async Task<JsonResult> OnGetComments(string productId)
        {
            _logger.LogDebug("Trying to load comments for product: " + productId);
            using var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{GlobalVariables.GATEWAY}{GlobalVariables.GET_COMMENTS_BY_PRODUCT}{productId}");
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
