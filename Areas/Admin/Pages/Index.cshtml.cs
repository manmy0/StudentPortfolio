using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace StudentPortfolio.Areas.Admin.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
