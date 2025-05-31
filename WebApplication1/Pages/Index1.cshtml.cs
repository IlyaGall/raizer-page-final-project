using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApplication1.Pages
{

    [Authorize] // тестируем авторизацию
    public class Index1Model : PageModel
    {
        public void OnGet()
        {
        }
    }
}
