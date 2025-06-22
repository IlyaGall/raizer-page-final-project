using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Model.Product;
using Model.Cluster;
using AuthService.Dto;
using GlobalVariablesRP;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using WebApplication1.Model;
using ClusterService;
using System.Collections.Generic;
using WebApplication1.Model.Product.ProductDto;
using System.Text.RegularExpressions;

namespace WebApplication1.Pages
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _client;
     
        public string Message { get; }

        private readonly ILogger<IndexModel> _logger;


        public IndexModel(IConfiguration configuration, IHttpClientFactory clientFactory, ILogger<IndexModel> logger)
        {
           
            _client = clientFactory.CreateClient();
            _client.BaseAddress = new Uri(GlobalVariables.GETWAY_OCELOT);
            _logger = logger;
            Message = "�����:";
        }

     

        public string PrintTime() => DateTime.Now.ToLongTimeString();

        public void OnGet()
        {

        }


        /// <summary>
        /// ����� ��� AJAX-������� ��� �������
        /// </summary>
        /// <returns></returns>
        public IActionResult OnGetCurrentTime()
        {
            return new JsonResult(new { time = DateTime.Now.ToLongTimeString() });
        }

        /// <summary>
        /// ��������� ���������� ������� �� �������� index
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<JsonResult> OnGetSearch(string query)
        {

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true // ���� � JSON camelCase (��������, "id" ������ "Id")
            };
            List<ProductDto> products1 = new List<ProductDto>();

            
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["JWTToken"]);

            var response = await _client.GetAsync(
                $"{GlobalVariables.GETWAY_OCELOT}" +
                $"{GlobalVariables.GET_SEARCH_KEYWORD_CLUSTER}{query}"); 

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    // ���������� Unicode-������������������
                    string decodedJson = System.Text.RegularExpressions.Regex.Unescape(responseBody);
                    string decodedString = System.Text.RegularExpressions.Regex.Unescape(decodedJson);
                    // 2 ���� � ������� �� �����!

                    string cleanJson = Regex.Unescape(decodedString.Trim('"'));
                    List<ClusterDto> items = JsonSerializer.Deserialize<List<ClusterDto>>(cleanJson, options);

                    /// ������ � ����������

                    var options1 = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping // �����!
                    };
                    foreach(var item in items)
                    {
                        var responseProducts = await _client.GetAsync(
                      $"{GlobalVariables.GETWAY_OCELOT}" +
                      $"{GlobalVariables.GET_PRODUCTS_BY_CLUSTER}{item.Id}");
                        if (responseProducts.IsSuccessStatusCode)
                        {
                            string responseBodyProduct = await responseProducts.Content.ReadAsStringAsync();
                             responseBody = System.Web.HttpUtility.HtmlDecode(responseBodyProduct);
                            //�������� � �������� ������� ��� ��� ���� ������� ����������� ��� ���� � ������������ ����
                            // ����� �������������� JSON 
                            var itemsProducts = JsonSerializer.Deserialize<List<ProductDto>>(responseBody, options);


                            products1.AddRange(itemsProducts);

                        }
                    }
                    // ����� �������������� JSON 
                    var distinctItems = products1.GroupBy(x => x.ProductId).Select(y => y.First());
                    var result = distinctItems.Select(item => new Product
                    {
                        Id = item.ProductId,
                        Name = item.NameProduct,
                        Description = item.Description,
                        Price = item.Price

                    })
                     .ToList();
                    return new JsonResult(result);
                }
                catch (Exception ex) 
                {
                
                }
                return new JsonResult("");
            }
            return new JsonResult("");
        }

        /// <summary>
        /// ������ � ���������� ������ ��� ����
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> OnGetCategories()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["JWTToken"]);

            var response = await _client.GetAsync(
                $"{GlobalVariables.GETWAY_OCELOT}" +
                $"{GlobalVariables.GET_CLUSTER}"); //������ �� �����, ��� ��� ������� jwt �����
            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();


                // ����� �������������� JSON 
                var items = JsonSerializer.Deserialize<List<ClusterDto>>(responseBody);

                var result = items.Select(item => new 
                {
                    Id = item.Id,
                    Name = item.Name,
                    ParentId = item.ParentId,
                 
                })
                 .ToList();
                return new JsonResult(result);
            }
            return new JsonResult("");
        }

       
        /// <summary>
        /// �������� �������� �� id ��������
        /// </summary>
        /// <param name="categoryId">id ��������</param>
        /// <returns></returns>
        public async Task<JsonResult> OnGetProductsByCategory(int categoryId)
        {
            List<ProductDto> products1 = new List<ProductDto>();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping // �����!
            };
            var responseProducts = await _client.GetAsync(
               $"{GlobalVariables.GETWAY_OCELOT}" +
               $"{GlobalVariables.GET_PRODUCTS_BY_CLUSTER}{categoryId}");
            if (responseProducts.IsSuccessStatusCode)
            {
                string responseBodyProduct = await responseProducts.Content.ReadAsStringAsync();
                var  responseBody = System.Web.HttpUtility.HtmlDecode(responseBodyProduct);
                //�������� � �������� ������� ��� ��� ���� ������� ����������� ��� ���� � ������������ ����
                // ����� �������������� JSON 
                var itemsProducts = JsonSerializer.Deserialize<List<ProductDto>>(responseBody, options);


                products1.AddRange(itemsProducts);

            }



            var response = await _client.GetAsync(
                $"{GlobalVariables.GETWAY_OCELOT}" +
                $"{GlobalVariables.GET_SERCH_CHILDREN_CLUSTER}{categoryId}");
          
            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                // ����� �������������� JSON 
                var items = JsonSerializer.Deserialize<List<ClusterDto>>(responseBody);

               

               

                foreach (var item in items) 
                {
                     responseProducts = await _client.GetAsync(
                  $"{GlobalVariables.GETWAY_OCELOT}" +
                  $"{GlobalVariables.GET_PRODUCTS_BY_CLUSTER}{item.Id}");
                    
                    if (response.IsSuccessStatusCode)
                    {
                        string responseBodyProduct = await responseProducts.Content.ReadAsStringAsync();
                        responseBody = System.Web.HttpUtility.HtmlDecode(responseBodyProduct);
                        //�������� � �������� ������� ��� ��� ���� ������� ����������� ��� ���� � ������������ ����
                        // ����� �������������� JSON 
                        var itemsProducts = JsonSerializer.Deserialize<List<ProductDto>>(responseBody, options);
                       
                       
                        products1.AddRange(itemsProducts);

                    }
                }
        
            }
            var distinctItems = products1.GroupBy(x => x.ProductId).Select(y => y.First());
            var result = distinctItems.Select(item => new Product
            {
                Id = item.ProductId,
                Name = item.NameProduct,
                Description = item.Description,
                Price = item.Price

            })
             .ToList();
            return new JsonResult(result);
           
        }






    }
}
