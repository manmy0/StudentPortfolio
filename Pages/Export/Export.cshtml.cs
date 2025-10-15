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

        [BindProperty]
        public bool ExportSummary {  get; set; }

        [BindProperty]
        public bool ExportPitch { get; set; }

        [BindProperty]
        public bool ExportCompetencies { get; set; }

        [BindProperty]
        public bool ExportResume { get; set; }

        [BindProperty]
        public bool ExportCoverLetter { get; set; }

        [BindProperty]
        public bool ExportGoals { get; set; }

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

        // method to clean up the data from the database so it can be exported
        public static string CleanCSV(string value)
        {
            if (string.IsNullOrEmpty(value))
                return "";

            // double any quote
            value = value.Replace("\"", "\"\"");

            // wrap a comma, quote or new line in quotes
            if (value.Contains(',') || value.Contains('"') || value.Contains('\n') || value.Contains('\r'))
            {
                value = $"\"{value}\"";
            }

            return value;
        }

        public async Task<IActionResult> OnPostExportPortfolio()
        {
            // have to grab the user again to get the right information
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return RedirectToPage("/Account/Login");
            }

            // if not null then we can grab the info we need
            CurrentUser = await _userManager.FindByIdAsync(userId);

            var sb = new StringBuilder();

            if (ExportSummary)
            {
                var summary = CurrentUser.Introduction ?? "No personal summary found.";

                sb.AppendLine("Personal Summary");
                sb.AppendLine(CleanCSV(summary));
                sb.AppendLine();
            }

            if (ExportPitch)
            {
                var pitch = CurrentUser.Pitch ?? "No elevator pitch found.";

                sb.AppendLine("Elevator Pitch");
                sb.AppendLine(CleanCSV(pitch));
                sb.AppendLine();
            }

            // just refresh the page if nothing was exported
            if (sb.Length == 0)
            {
                return RedirectToPage();
            }
            

            // encode it to a byte array so it can be exported
            var bytes = Encoding.UTF8.GetPreamble()
                .Concat(Encoding.UTF8.GetBytes(sb.ToString()))
                .ToArray();

            return File(bytes, "text/csv", "Portfolio_Export.csv");
        }
    }
}
