using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace StudentPortfolio.Areas.Admin.Pages
{
    public class LoginModel : PageModel
    {
        public IActionResult OnGet(string? returnUrl = null)
        {
            var identityLoginPath = "/Identity/Account/Login";

            if (!string.IsNullOrEmpty(returnUrl))
            {
                identityLoginPath += $"?returnUrl={Uri.EscapeDataString(returnUrl)}";
            }

            return LocalRedirect(identityLoginPath);
        }
    }
}
