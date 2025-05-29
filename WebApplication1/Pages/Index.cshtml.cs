using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

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


        //  ����� ��� AJAX-�������
        public IActionResult OnGetCurrentTime()
        {
            return new JsonResult(new { time = DateTime.Now.ToLongTimeString() });
        }

        public JsonResult OnGetSearch(string query)
        {
            // ����� ���������� ������ ������ �������
            // ������:
            var searchResults = new List<Product>
        {
            new Product { Id = 1, Name = query + " 1", Description = "�������� " + query + " 1", Price = 100 },
            new Product { Id = 2, Name = query + " 2", Description = "�������� " + query + " 2", Price = 200 },
            new Product { Id = 3, Name = query + " 3", Description = "�������� " + query + " 3", Price = 300 }
        };

            return new JsonResult(searchResults);
        }


        public JsonResult OnGetCategories()
        {
            // ������ ������ - � ���������� �� ������ �������� �� �� ��
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

        public class Category
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int ParentId { get; set; }
        }

        public class Product
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public decimal Price { get; set; }
        }


    }
}
