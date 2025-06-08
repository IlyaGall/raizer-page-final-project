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
            Message = "Время:";
        }

        public string PrintTime() => DateTime.Now.ToLongTimeString();

        public void OnGet()
        {

        }


        /// <summary>
        /// метод для AJAX-запроса для часиков
        /// </summary>
        /// <returns></returns>
        public IActionResult OnGetCurrentTime()
        {
            return new JsonResult(new { time = DateTime.Now.ToLongTimeString() });
        }

        /// <summary>
        /// Обработка поискового запроса на странице index
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public JsonResult OnGetSearch(string query)
        {
            // Здесь логика поиска товаров
            // Пример:
            var searchResults = new List<Product>
            {
                new Product { Id = 1, Name = query + " 1", Description = "Описание " + query + " 1", Price = 100 },
                new Product { Id = 2, Name = query + " 2", Description = "Описание " + query + " 2", Price = 200 },
                new Product { Id = 3, Name = query + " 3", Description = "Описание " + query + " 3", Price = 300 }
            };

            return new JsonResult(searchResults);
        }

        /// <summary>
        /// Запрос к категориям товара для меню
        /// </summary>
        /// <returns></returns>
        public JsonResult OnGetCategories()
        {
            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Продукты", ParentId = -1 },
                new Category { Id = 2, Name = "Мясо", ParentId = 1 },
                new Category { Id = 3, Name = "Овощи", ParentId = 1 },
                new Category { Id = 4, Name = "Электроника", ParentId = -1 },
                new Category { Id = 5, Name = "Ноутбуки", ParentId = 4 },
                new Category { Id = 6, Name = "Телефоны", ParentId = 4 },
                new Category { Id = 10, Name = "Apple", ParentId = 6 },
                new Category { Id = 11, Name = "Sumsung", ParentId = 6 },
                new Category { Id = 12, Name = "Honor", ParentId = 6 },

                new Category { Id = 7, Name = "Молочные продукты", ParentId = 1 },
                new Category { Id = 9, Name = "Без категории", ParentId = -1 },
            };

            return new JsonResult(categories);

        }

       
        /// <summary>
        /// Получить продукты по id кластеру
        /// </summary>
        /// <param name="categoryId">id кластера</param>
        /// <returns></returns>
        public JsonResult OnGetProductsByCategory(int categoryId)
        {
            var products = new List<Product>();
            if (categoryId == 2) // Мясо
            {
                products.AddRange(new List<Product>
                {
                    new Product { Id = 101, Name = "Говядина", Description = "Свежая говядина", Price = 500 },
                    new Product { Id = 102, Name = "Свинина", Description = "Свежая свинина", Price = 450 }
                });
            }
            else if (categoryId == 3) // Овощи
            {
                products.AddRange(new List<Product>
                {
                    new Product { Id = 201, Name = "Морковь", Description = "Свежая морковь", Price = 50 },
                    new Product { Id = 202, Name = "Картофель", Description = "Свежий картофель", Price = 40 }
                });
            }
            else if (categoryId == 9) // Без категории
            {
                products.AddRange(new List<Product>
                {
                    new Product { Id = 301, Name = "Товар без категории 1", Description = "Описание", Price = 100 },
                    new Product { Id = 302, Name = "Товар без категории 2", Description = "Описание", Price = 200 }
                });
            }
            return new JsonResult(products);
        }






    }
}
