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
        /// �������� ���������� � ������
        /// </summary>
        public void LoadInfoProduct()
        {

        }

        /// <summary>
        /// API endpoint ��� ��������� ���������� � ������
        /// </summary>
        public async Task<IActionResult> OnGetProductInfoAsync(string id)
        {
            // ����� �� ��������� ������ � ������ �� ���� ������ ��� ������� ���������

            var productInfo = await LoadInfoProduct(id);

            return new JsonResult(productInfo);
        }

        /// <summary>
        /// �������� ���������� � ������
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
        /// ���������� �������� � �������
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken] // ������� �� ��������� ������� ��� �������� ������
        public async Task<IActionResult> OnPostAddCartProduct([FromBody] AddCartDto request)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            };
            try
            {
                // 1. �������� ��������������
                if (!User.Identity.IsAuthenticated)
                {
                    return BadRequest(new { message = "��������� �����������" });
                }

                // 2. ������� ���������
                //if (string.IsNullOrWhiteSpace(request.UserName))
                {
                    //return BadRequest(new { message = "��� ������������ �����������" });
                }

                if (request.UserId <= 0)
                {
                    return BadRequest(new { message = "������������ ID ������������" });
                }

                // 3. �������� ������������� ������������
                var responseUser = await _client.GetAsync(
                    $"{GlobalVariables.GETWAY_OCELOT}{GlobalVariables.GET_USER_ID}{request.UserId}");

                if (!responseUser.IsSuccessStatusCode)
                {
                    return BadRequest(new { message = "������������ � ��������� ID �� ������" });
                }

                // 4. �������� ������������ ������
                var responseBody = await responseUser.Content.ReadAsStringAsync();
                var item = JsonSerializer.Deserialize<UserEasyDto>(responseBody, options);

                //if (item.Id != request.UserId || item.Login != request.UserName)
                {
                    //return BadRequest(new { message = "ID ��� ����� ������������ �� ���������. ������������ �� �������!" });
                }

                // 5. ���������� ���������
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
                return BadRequest(new { message = $"��������� ������: {ex.Message}" });
            }
        }
    }
}
