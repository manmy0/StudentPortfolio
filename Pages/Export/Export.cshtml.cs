using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentPortfolio.Models;
using System.Security.Claims;
using System.Text;

namespace StudentPortfolio.Pages.Export
{
    [Authorize]
    public class ExportModel : PageModel
    {
        private readonly StudentPortfolio.Data.ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ApplicationUser CurrentUser { get; set; }

        public ExportModel(StudentPortfolio.Data.ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task OnGetAsync()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId != null)
            {
                CurrentUser = await _userManager.FindByIdAsync(userId);
            }

        }

        public async Task<IActionResult> OnPostExportSummaryAsCSV()
        {
            // have to grab the user again to get the right information
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return RedirectToPage("/Account/Login");
            }

            // if not null then we can grab the info we need
            CurrentUser = await _userManager.FindByIdAsync(userId);

            var summary = CurrentUser.Introduction;

            // heading of personal summary, underneat it append their summary
            // or that there is no summary provided
            var sb = new StringBuilder();
            sb.AppendLine("Personal Summary");

            sb.AppendLine(summary ?? "No summary provided.");

            // encode it to a byte array so it can be exported
            var bytes = Encoding.UTF8.GetPreamble()
                .Concat(Encoding.UTF8.GetBytes(sb.ToString()))
                .ToArray();

            return File(bytes, "text/csv", "Personal_Summary.csv");
        }

        
    }
}
