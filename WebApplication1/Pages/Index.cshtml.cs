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
            Message = "Время:";
        }

        public string PrintTime() => DateTime.Now.ToLongTimeString();

        public void OnGet()
        {

        }


        // Новый метод для AJAX-запроса
        public IActionResult OnGetCurrentTime()
        {
            return new JsonResult(new { time = DateTime.Now.ToLongTimeString() });
        }

        public JsonResult OnGetSearch(string query)
        {
            // Здесь реализуйте логику поиска товаров
            // Пример:
            var searchResults = new List<Product>
        {
            new Product { Id = 1, Name = query + " 1", Description = "Описание " + query + " 1", Price = 100 },
            new Product { Id = 2, Name = query + " 2", Description = "Описание " + query + " 2", Price = 200 },
            new Product { Id = 3, Name = query + " 3", Description = "Описание " + query + " 3", Price = 300 }
        };

            return new JsonResult(searchResults);
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
