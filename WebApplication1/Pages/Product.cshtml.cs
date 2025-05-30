using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApplication1.Pages
{
    // https://localhost:7100/Product?name=ds
    public class ProductModel : PageModel
    {
        public string Message { get; private set; } = "";
        public void OnGet(string id)
        {
            Message = $"Id: {id}";
        }
    }
}
