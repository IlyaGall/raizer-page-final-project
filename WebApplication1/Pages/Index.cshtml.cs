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
    }
}
