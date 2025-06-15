using GlobalVariablesRP;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using ShopService.Domain;
using System.Net.Http.Headers;
using System.Text.Json;
using WebApplication1.Model.Shop;

namespace WebApplication1.Pages
{
    public class ShopManagementModel : PageModel
    {
        private readonly HttpClient _client;

        public ShopManagementModel(IHttpClientFactory clientFactory)
        {
            _client = clientFactory.CreateClient();
            _client.BaseAddress = new Uri(GlobalVariables.GETWAY_OCELOT);
        }


        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        public Shop Shop { get; set; }

        public async  Task OnGetAsync()
        {

           // _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["JWTToken"]);
            var response = await _client.GetAsync(
                $"{GlobalVariables.GETWAY_OCELOT}" +
                $"{GlobalVariables.GET_INFO_SHOP}{Id}");

            if (response.IsSuccessStatusCode)
            {
                //TempData["SuccessMessage"] = "������ ������� ���������";
                string responseBody = await response.Content.ReadAsStringAsync();
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
            }
            else
            {
            
                TempData["SuccessMessage"] += "������ ��� ���������� ������ �� ��������";
               
            }
            // �������� ���������� � �������� �� ��
            // Shop = await _shopService.GetShopByIdAsync(Id);

            // ��������� mock ������
            //Shop = new Shop
            //{
            //    Id = Id,
            //    Name = "��� �������",
            //    Description = "�������� ��������",
            //    Adress = "��. ���������, 123",
            //    ContactInfo = "contact@example.com"
            //};
        }

        public async Task<JsonResult> OnGetProducts(int shopId)
        {
            // �������� ������ �������� �� ��
            // var products = await _productService.GetProductsByShopAsync(shopId);

            var products = new[]
            {
                new { Id = 1, Name = "����� 1", Price = 1000 },
                new { Id = 2, Name = "����� 2", Price = 2000 }
            };

            return new JsonResult(products);
        }

        public async Task<JsonResult> OnGetManagers(int shopId)
        {
            // �������� ���������� �������� �� ��
            // var managers = await _shopService.GetShopManagersAsync(shopId);

            var managers = new[]
            {
                new { Id = 1, Name = "���� ������", Email = "ivan@example.com" },
                new { Id = 2, Name = "���� ������", Email = "petr@example.com" }
            };

            return new JsonResult(managers);
        }

        public async Task<IActionResult> OnPostAddManagerAsync([FromBody] AddManagerRequest request)
        {
            try
            {
                // ��������� ��������� � �������
                // await _shopService.AddManagerAsync(request.ShopId, request.Email);
                return new OkResult();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public async Task<IActionResult> OnPostDeleteProductAsync(int productId)
        {
            try
            {
                // ������� �����
                // await _productService.DeleteProductAsync(productId);
                return new OkResult();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public async Task<IActionResult> OnPostRemoveManagerAsync(int managerId, int shopId)
        {
            try
            {
                // ������� ��������� �� ��������
                // await _shopService.RemoveManagerAsync(shopId, managerId);
                return new OkResult();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

    public class AddManagerRequest
    {
        public int ShopId { get; set; }
        public string Email { get; set; }
    }
}