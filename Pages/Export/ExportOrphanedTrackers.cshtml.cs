using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentPortfolio.Models;
using System.Collections;
using System.IO.Compression;
using System.Security.Claims;
using System.Text;

namespace StudentPortfolio.Pages.Export
{
    [Authorize]
    public class ExportOrphanedTrackersModel : PageModel
    {
        private readonly StudentPortfolio.Data.ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ApplicationUser CurrentUser { get; set; }

        [BindProperty]
        public bool ExportCompetencies { get; set; }

        public IList<CompetencyTracker> Trackers { get; set; } = default!;

        public async Task OnGetAsync()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId != null)
            {
                CurrentUser = await _userManager.FindByIdAsync(userId);
            }

            await GetOrphanedTrackersAsync(userId);
            

        }

        // method to clean up the data from the database so it can be exported
        // Same as Export.cshtml.cs
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

        private async Task GetOrphanedTrackersAsync(string userId)
        {
            //var sbCompetencies = new StringBuilder();

            IList<CompetencyTracker> OrphanedTrackers = null;

            var competencyTrackers = await _context.CompetencyTrackers
                    .Where(c => c.UserId == userId)
                    .ToListAsync();

            var competencies = await _context.Competencies
                    .ToListAsync();

            foreach (var tracker in competencyTrackers)
            {
                if (!competencies.Where(c => c.CompetencyId == tracker.CompetencyId).Any())
                {
                    OrphanedTrackers.Add(tracker);
                }
            }

            if (OrphanedTrackers.Any())
            {
                Trackers = OrphanedTrackers;
            }

        }

        private async Task<StringBuilder> GetCompetenciesCsvAsync(string userId)
        {
            var sbCompetencies = new StringBuilder();

            await GetOrphanedTrackersAsync(userId);

            if (!Trackers.Any())
            {
                return null;
            }

            var exportTrackers = Trackers
                .Select(c => new
                {
                    c.CompetencyTrackerId,
                    LevelDescription = c.Level.Name,
                    c.SkillsReview,
                    c.Evidence,
                    c.StartDate,
                    c.EndDate,
                    c.Created,
                    c.LastUpdated
                })
                .ToList();
                
            if (exportTrackers.Any())
            {
                sbCompetencies.AppendLine("\"Competency Tracker Id\",\"Level\",\"Skills Review\",\"Evidence\",\"Start Date\",\"End Date\",\"Created\",\"Last Updated\"");
                foreach (var comp in exportTrackers)
                {
                    sbCompetencies.AppendLine(
                        $"{CleanCSV(comp.CompetencyTrackerId.ToString())}," +
                        $"{CleanCSV(comp.LevelDescription)}," +
                        $"{CleanCSV(comp.SkillsReview)}," +
                        $"{CleanCSV(comp.Evidence)}," +
                        $"{CleanCSV(comp.StartDate.ToString())}," +
                        $"{CleanCSV(comp.EndDate.ToString())}," +
                        $"{CleanCSV(comp.Created.ToString())}," +
                        $"{CleanCSV(comp.LastUpdated.ToString())}"
                    );
                }
            }

            return sbCompetencies;
        }

        // takes exportData which has name of file and data to be written to it
        private IActionResult CreateZipFile(Dictionary<string, StringBuilder> exportData)
        {
            // create a new memory stream object to temporarily hold the zip file while its being built
            using (var memoryStream = new MemoryStream())
            {

                // create a zip archive object to build the zip
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {

                    // loop through all the files that need to be created from dict
                    foreach (var kvp in exportData)
                    {
                        string fileName = kvp.Key;
                        StringBuilder content = kvp.Value;

                        if (content.Length > 0)
                        {

                            // create new file
                            var entry = archive.CreateEntry(fileName);
                            using (var entryStream = entry.Open())
                            using (var writer = new StreamWriter(entryStream, Encoding.UTF8))
                            {
                                // write to the file
                                writer.Write(content.ToString());
                            }
                        }
                    }
                }

                // export the zip file
                return File(memoryStream.ToArray(), "application/zip", "Portfolio_Export.zip");
            }
        }

        public async Task<IActionResult> OnPostExportPortfolio()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToPage("/Account/Login");
            }

            CurrentUser = await _userManager.FindByIdAsync(userId);

            var exportData = new Dictionary<string, StringBuilder>();

            if (ExportCompetencies)
            {
                var competenciesSb = await GetCompetenciesCsvAsync(userId);
                if (competenciesSb.Length > 0)
                    exportData.Add("Competencies.csv", competenciesSb);
            }


            if (exportData.Count == 0)
            {
                return RedirectToPage();
            }

            return CreateZipFile(exportData);
        }
    }
}
