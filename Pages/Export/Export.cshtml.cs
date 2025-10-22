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

        [BindProperty]
        public bool ExportCDP { get; set; }

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

        private StringBuilder GetSummaryCsv()
        {
            var sb = new StringBuilder();
            var summary = CurrentUser.Introduction ?? "No personal summary found.";

            sb.AppendLine("Personal Summary");
            sb.AppendLine(CleanCSV(summary));
            sb.AppendLine();
            return sb;
        }

        private StringBuilder GetPitchCsv()
        {
            var sb = new StringBuilder();
            var pitch = CurrentUser.Pitch ?? "No elevator pitch found.";

            sb.AppendLine("Elevator Pitch");
            sb.AppendLine(CleanCSV(pitch));
            sb.AppendLine();
            return sb;
        }

        private async Task<StringBuilder> GetCompetenciesCsvAsync(string userId)
        {
            var sbCompetencies = new StringBuilder();

            var competencies = await _context.CompetencyTrackers
                .Where(c => c.UserId == userId)
                .Select(c => new
                {
                    c.CompetencyTrackerId,
                    CompetencyDescription = c.Competency.Description,
                    LevelDescription = c.Level.Name,
                    c.SkillsReview,
                    c.Evidence,
                    c.StartDate,
                    c.EndDate,
                    c.Created,
                    c.LastUpdated
                })
                .ToListAsync();

            if (competencies.Any())
            {
                sbCompetencies.AppendLine("\"Competency Number\",\"Competency Description\",\"Level\",\"Skills Review\",\"Evidence\",\"Start Date\",\"End Date\",\"Created\",\"Last Updated\"");
                foreach (var comp in competencies)
                {
                    sbCompetencies.AppendLine(
                        $"{CleanCSV(comp.CompetencyTrackerId.ToString())}," +
                        $"{CleanCSV(comp.CompetencyDescription)}," +
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

        private async Task<StringBuilder> GetGoalsCsvAsync(string userId)
        {
            var sbGoals = new StringBuilder();

            var goals = await _context.Goals
                .Where(g => g.UserId == userId)
                .Select(g => new
                {
                    g.Description,
                    g.Timeline,
                    GoalSteps = g.GoalSteps.Select(gs => gs.Step).ToList(),
                    g.Progress,
                    g.Learnings,
                    g.StartDate,
                    g.EndDate,
                    g.DateSet,
                    g.CompleteDate,
                    g.CompletionNotes
                })
                .ToListAsync();

            if (goals.Any())
            {
                sbGoals.AppendLine("\"Goal\",\"Timeline\",\"Goal Steps\",\"Progress\",\"Learnings\",\"Start Date\",\"End Date\",\"Set Date\",\"Complete Date\",\"Completion Notes\"");
                foreach (var goal in goals)
                {
                    // make all the steps a single string separated by ;
                    string steps = string.Join("; ", goal.GoalSteps);

                    sbGoals.AppendLine(
                        $"{CleanCSV(goal.Description)}," +
                        $"{CleanCSV(goal.Timeline)}," +
                        $"{CleanCSV(steps)}," +
                        $"{CleanCSV(goal.Progress)}," +
                        $"{CleanCSV(goal.Learnings)}," +
                        $"{CleanCSV(goal.StartDate.ToString())}," +
                        $"{CleanCSV(goal.EndDate.ToString())}," +
                        $"{CleanCSV(goal.DateSet.ToString())}," +
                        $"{CleanCSV(goal.CompleteDate.ToString())}," +
                        $"{CleanCSV(goal.CompletionNotes)}"
                    );
                }
            }

            return sbGoals;
        }

        private async Task<StringBuilder> GetCDPCsvAsync(string userId)
        {
            var sbCDP = new StringBuilder();

            var CDP = await _context.CareerDevelopmentPlans
                .Where(c => c.UserId == userId)
                .Select(c => new
                {
                    c.Year,
                    c.ProfessionalInterests,
                    c.EmployersOfInterest,
                    c.PersonalValues,
                    c.DevelopmentFocus,
                    c.Extracurricular,
                    c.NetworkingPlan
                })
                .ToListAsync();

            if (CDP.Any())
            {
                sbCDP.AppendLine("\"Year\",\"Professional Interests\",\"Employers of Interest\",\"Personal Values\",\"Development Focus\",\"Extra Curricular\",\"Networking Plan\"");
                foreach (var cdp in CDP)
                {
                    sbCDP.AppendLine(
                        $"{CleanCSV(cdp.Year.ToString())}," +
                        $"{CleanCSV(cdp.ProfessionalInterests)}," +
                        $"{CleanCSV(cdp.EmployersOfInterest)}," +
                        $"{CleanCSV(cdp.PersonalValues)}," +
                        $"{CleanCSV(cdp.DevelopmentFocus)}," +
                        $"{CleanCSV(cdp.Extracurricular)}," +
                        $"{CleanCSV(cdp.NetworkingPlan)}"
                    );
                }
            }

            return sbCDP;
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

                // create the file name
                var user = CurrentUser.FirstName + "_" + CurrentUser.LastName;
                var date = DateTime.Now;
                var dateString = date.ToString("dd-MM-yyyy");
                var finalFileName = $"{user}_Portfolio_Export_{dateString}.zip";

                // export the zip file
                return File(memoryStream.ToArray(), "application/zip", finalFileName);
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

            // get the date and user name for the file name
            var user = CurrentUser.FirstName + "_" + CurrentUser.LastName;
            var date = DateTime.Now;
            var dateString = date.ToString("dd-MM-yyyy");

            if (ExportSummary)
            {
                var fileName = $"{user}_Personal_Summary_{dateString}.csv";
                exportData.Add(fileName, GetSummaryCsv());
            }

            if (ExportPitch)
            {
                var fileName = $"{user}_Elevator_Pitch_{dateString}.csv";
                exportData.Add(fileName, GetPitchCsv());
            }

            if (ExportCompetencies)
            {
                var competenciesSb = await GetCompetenciesCsvAsync(userId);
                if (competenciesSb.Length > 0)
                {
                    var fileName = $"{user}_Competencies_{dateString}.csv";
                    exportData.Add(fileName, competenciesSb);
                } 
            }

            if (ExportGoals)
            {
                var goalsSb = await GetGoalsCsvAsync(userId);
                if (goalsSb.Length > 0)
                {
                    var fileName = $"{user}_Goals_{dateString}.csv";
                    exportData.Add(fileName, goalsSb);
                }
            }

            if (ExportCDP)
            {
                var cdpSb = await GetCDPCsvAsync(userId);
                if (cdpSb.Length > 0)
                {
                    var fileName = $"{user}_CDP_{dateString}.csv";
                    exportData.Add(fileName, cdpSb);
                }
            }

            if (exportData.Count == 0)
            {
                return RedirectToPage();
            }

            return CreateZipFile(exportData);
        }
    }
}
