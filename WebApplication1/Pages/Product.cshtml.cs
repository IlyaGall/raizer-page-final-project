using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Model.Product;
using System.Security.AccessControl;
using ConnectBackEnd;
using WebApplication1.Model.Product.ProductDto;
using System.Xml.Linq;
using GlobalVariablesRP;

namespace WebApplication1.Pages
{
    // https://localhost:7100/Product?name=ds
    public class ProductModel : PageModel
    {
        public string Message { get; private set; } = "";

        private string IdProduct { get; set; } = string.Empty;
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


    }
}
