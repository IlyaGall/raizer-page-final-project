using GlobalVariablesRP;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Model.Product;
using ShopService.Domain;
using System.Net.Http.Headers;
using System.Text.Json;
using WebApplication1.Model.Product.ProductDto;
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
                TempData["SuccessMessage"] = "������ ������� ���������";
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


        #region ������ � ���������
        [HttpPost]
        [ValidateAntiForgeryToken] // ������� �� ��������� ������� ��� �������� ������
        public async Task<IActionResult> OnPostUpdateShop([FromBody] ShopUpdateRequest request)
        {
            // ����� ��� �������� �������������
            if (!ModelState.IsValid)
            {
                return Page();
            }
            try
            {
                var response = await _client.PutAsJsonAsync($"{GlobalVariables.GETWAY_OCELOT}shops/{request.Id}", request);
                if (response.IsSuccessStatusCode)
                {
                    return new OkResult();
                }
                return BadRequest(await response.Content.ReadAsStringAsync());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        #endregion



        #region  ������ � ��������

        /// <summary>
        /// �������� ������� ��� �������� �� ��
        /// </summary>
        /// <param name="shopId"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken] // ������� �� ��������� ������� ��� �������� ������
        public async Task<JsonResult> OnGetProducts(int shopId)
        {
            

            // �������� ������ �������� �� ��
            // var products = await _productService.GetProductsByShopAsync(shopId);
            // ����� ��� �������� �������������
            try
            {
                var response = await _client.GetAsync(
                  $"{GlobalVariables.GETWAY_OCELOT}" +
                  $"{GlobalVariables.GET_SHOP_PRODUCTS}{shopId}");
                string s= $"{GlobalVariables.GETWAY_OCELOT}"+
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


                    if (items == null || items.Count == 0)
                    {
                        return null;
                    }
                    var result = items.Select(item => new
                    {
                        Id = item.ShopId,
                        Name = item.NameProduct,
                        Price = item.Price
                    })
                     .ToList();


                    return new JsonResult(result);

                    foreach (var item in items)
                    {
                        
                    }
                }

            }
            catch (Exception ex)
            {
               // return BadRequest(ex.Message);
            }


            //var products = new[]
            //{
            //    new { Id = 1, Name = "����� 1", Price = 1000 },
            //    new { Id = 2, Name = "����� 2", Price = 2000 }
            //};

           // return new JsonResult(products);
            return null;
        }

        /// <summary>
        /// �������� ������ �� ��������
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
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

        /// <summary>
        /// ���������� ��������
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnPostUpdateProduct([FromBody] ProductDto request)
        {
            try
            {
                var response = await _client.PutAsJsonAsync($"{GlobalVariables.GETWAY_OCELOT}products/{request.ProductId}", request);
                if (response.IsSuccessStatusCode)
                {
                    return new OkResult();
                }
                return BadRequest(await response.Content.ReadAsStringAsync());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// ���������� ��������
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnPostSaveProductAsync([FromBody] ProductSave product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // 1. �������� ����������� � ����
                if (!User.Identity.IsAuthenticated)
                {
                    return Unauthorized();
                }

                // 2. ��������� ������
                if (string.IsNullOrWhiteSpace(product.Name) || product.Price <= 0)
                {
                    return BadRequest("�������� � ���� ������ �����������");
                }

                // 3. ������ ���������� (������ � HttpClient)
                _client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", Request.Cookies["JWTToken"]);

                var response = await _client.PostAsJsonAsync(
                    $"{GlobalVariables.GETWAY_OCELOT}/products",
                    new
                    {
                        product.Name,
                        product.Price,
                        product.Description,
                        ShopId = product.ShopId
                    });

                if (!response.IsSuccessStatusCode)
                {
                    return BadRequest(await response.Content.ReadAsStringAsync());
                }

                // 4. ���������� �������� ���������
                return new JsonResult(new { success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"������ ��� ���������� ������: {ex.Message}");
            }
        }
        #endregion


        #region ���������
        /// <summary>
        /// �������� ���������� ��������
        /// </summary>
        /// <param name="shopId"></param>
        /// <returns></returns>
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

        /// <summary>
        /// �������� ���������
        /// </summary>
        /// <param name="managerId"></param>
        /// <param name="shopId"></param>
        /// <returns></returns>
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

        /// <summary>
        /// ���������� ������ ���������
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
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
        
        #endregion
    }


    public class AddManagerRequest
    {
        public int ShopId { get; set; }
        public string Email { get; set; }
    }
    public class ShopUpdateRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string ContactInfo { get; set; }
    }


    public class Products 
    {
        int Id { get; set; }

        string Name { get; set; }

        double Price { get; set; }
    }

    public class ProductCreateRequest
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public int ShopId { get; set; }
    }

    public class ProductUpdateRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
    }

    public class ProductSave
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public int ShopId { get; set; }
    }
}