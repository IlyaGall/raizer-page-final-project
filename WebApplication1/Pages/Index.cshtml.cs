using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Model.Product;
using Model.Cluster;

namespace WebApplication1.Pages
{
    public class IndexModel : PageModel
    {
        public string Message { get; }

        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
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
        public JsonResult OnGetSearch(string query)
        {
            // ����� ������ ������ �������
            // ������:
            var searchResults = new List<Product>
            {
                new Product { Id = 1, Name = query + " 1", Description = "�������� " + query + " 1", Price = 100 },
                new Product { Id = 2, Name = query + " 2", Description = "�������� " + query + " 2", Price = 200 },
                new Product { Id = 3, Name = query + " 3", Description = "�������� " + query + " 3", Price = 300 }
            };

            return new JsonResult(searchResults);
        }

        /// <summary>
        /// ������ � ���������� ������ ��� ����
        /// </summary>
        /// <returns></returns>
        public JsonResult OnGetCategories()
        {
            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "��������", ParentId = -1 },
                new Category { Id = 2, Name = "����", ParentId = 1 },
                new Category { Id = 3, Name = "�����", ParentId = 1 },
                new Category { Id = 4, Name = "�����������", ParentId = -1 },
                new Category { Id = 5, Name = "��������", ParentId = 4 },
                new Category { Id = 6, Name = "��������", ParentId = 4 },
                new Category { Id = 10, Name = "Apple", ParentId = 6 },
                new Category { Id = 11, Name = "Sumsung", ParentId = 6 },
                new Category { Id = 12, Name = "Honor", ParentId = 6 },

                new Category { Id = 7, Name = "�������� ��������", ParentId = 1 },
                new Category { Id = 9, Name = "��� ���������", ParentId = -1 },
            };

            return new JsonResult(categories);

        }

       
        /// <summary>
        /// �������� �������� �� id ��������
        /// </summary>
        /// <param name="categoryId">id ��������</param>
        /// <returns></returns>
        public JsonResult OnGetProductsByCategory(int categoryId)
        {
            var products = new List<Product>();
            if (categoryId == 2) // ����
            {
                products.AddRange(new List<Product>
                {
                    new Product { Id = 101, Name = "��������", Description = "������ ��������", Price = 500 },
                    new Product { Id = 102, Name = "�������", Description = "������ �������", Price = 450 }
                });
            }
            else if (categoryId == 3) // �����
            {
                products.AddRange(new List<Product>
                {
                    new Product { Id = 201, Name = "�������", Description = "������ �������", Price = 50 },
                    new Product { Id = 202, Name = "���������", Description = "������ ���������", Price = 40 }
                });
            }
            else if (categoryId == 9) // ��� ���������
            {
                products.AddRange(new List<Product>
                {
                    new Product { Id = 301, Name = "����� ��� ��������� 1", Description = "��������", Price = 100 },
                    new Product { Id = 302, Name = "����� ��� ��������� 2", Description = "��������", Price = 200 }
                });
            }
            return new JsonResult(products);
        }






    }
}
