using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentPortfolio.Models;
using System.IO.Compression;
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

            var sbSummary = new StringBuilder();
            var sbPitch = new StringBuilder();
            var sbCompetencies = new StringBuilder();

            if (ExportSummary)
            {
                var summary = CurrentUser.Introduction ?? "No personal summary found.";

                sbSummary.AppendLine("Personal Summary");
                sbSummary.AppendLine(CleanCSV(summary));
                sbSummary.AppendLine();
            }

            if (ExportPitch)
            {
                var pitch = CurrentUser.Pitch ?? "No elevator pitch found.";

                sbPitch.AppendLine("Elevator Pitch");
                sbPitch.AppendLine(CleanCSV(pitch));
                sbPitch.AppendLine();
            }

            if (ExportCompetencies)
            {
                var competencies = await _context.CompetencyTrackers
                    .Where(c => c.UserId == userId)
                    .Select(c => new
                    {
                        c.CompetencyTrackerId,
                        c.CompetencyId,
                        c.StartDate,
                        c.EndDate,
                        c.LevelId,
                        c.SkillsReview,
                        c.Evidence,
                        c.Created,
                        c.LastUpdated
                    })
                    .ToListAsync();

                sbCompetencies.AppendLine("\"Competency Tracker ID\",\"Competency ID\",\"Level ID\",\"Skills Review\",\"Evidence\",\"Start Date\",\"End Date\",\"Created\",\"Last Updated\"");
                foreach (var comp in competencies)
                {
                    sbCompetencies.AppendLine(
                        $"{CleanCSV(comp.CompetencyTrackerId.ToString())}," +
                        $"{CleanCSV(comp.CompetencyId.ToString())}," +
                        $"{CleanCSV(comp.LevelId.ToString())}," +
                        $"{CleanCSV(comp.SkillsReview)}," +
                        $"{CleanCSV(comp.Evidence)}," +
                        $"{CleanCSV(comp.StartDate.ToString())}," +
                        $"{CleanCSV(comp.EndDate.ToString())}," +
                        $"{CleanCSV(comp.Created.ToString())}," +
                        $"{CleanCSV(comp.LastUpdated.ToString())}"
                    );
                }
            }

            // just refresh the page if nothing was exported
            if (sbSummary.Length == 0 && sbPitch.Length == 0 && sbCompetencies.Length == 0)
            {
                return RedirectToPage();
            }


            // create a new memory stream object to temporarily hold the zip file while its being built
            using (var memoryStream = new MemoryStream())
            {
                // create a zip archive object to build the zip
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    if (sbSummary.Length > 0)
                    {
                        // create new csv file
                        var entry = archive.CreateEntry("Personal_Summary.csv");
                        using (var entryStream = entry.Open())
                        using (var writer = new StreamWriter(entryStream, Encoding.UTF8))
                        {
                            // write to the file
                            writer.Write(sbSummary.ToString());
                        }
                    }

                    if (sbPitch.Length > 0)
                    {
                        var entry = archive.CreateEntry("Elevator_Pitch.csv");
                        using (var entryStream = entry.Open())
                        using (var writer = new StreamWriter(entryStream, Encoding.UTF8))
                        {
                            writer.Write(sbPitch.ToString());
                        }
                    }

                    if (sbCompetencies.Length > 0)
                    {
                        var entry = archive.CreateEntry("Competencies.csv");
                        using (var entryStream = entry.Open())
                        using (var writer = new StreamWriter(entryStream, Encoding.UTF8))
                        {
                            writer.Write(sbCompetencies.ToString());
                        }
                    }
                }

                // export the zip file
                return File(memoryStream.ToArray(), "application/zip", "Portfolio_Export.zip");
            }
        }
    }
}
