using ConnectBackEnd;
using GlobalVariablesRP;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using WebApplication1.Model.Comment;
using WebApplication1.Model.Product.ProductDto;

namespace WebApplication1.Pages
{
    // https://localhost:7100/Product?name=ds
    public class ProductModel : PageModel
    {
        public string Message { get; private set; } = "";

        private string IdProduct { get; set; } = string.Empty;
        private readonly ILogger _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly HttpClient _client;
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

        public async Task<JsonResult> OnGetComments(string productId)
        {
            _logger.LogDebug("Trying to load comments for product: " + productId);
            using var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{GlobalVariables.GATEWAY}{GlobalVariables.GET_COMMENTS_BY_PRODUCT}{productId}");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogInformation($"Failed to load comments: {response.Content}");
                return new JsonResult(new { error = "Error" });
            }

            var content = await response.Content.ReadAsStringAsync();
            _logger.LogDebug("Comments response:" + content);
            var comments = JsonSerializer.Deserialize<CommentModel[]>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            });
            return new JsonResult(comments);
            /*
            using var apiClient = new ConnectServer();
            var response = await apiClient.GetAsync<List<CommentModel>>(GlobalVariables.GET_COMMENTS_BY_PRODUCT + productId);
            if (!response.IsSuccess)
            {
                _logger.LogInformation($"Failed to load comments {response.ErrorMessage}");
                return new JsonResult(response.ErrorMessage);
            }
            _logger.LogInformation($"Loaded {response.Data.Count()} comments for product {productId}");
            return new JsonResult(response);
            */
        }
    }
}
