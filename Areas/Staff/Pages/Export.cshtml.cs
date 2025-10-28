using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentPortfolio.Models;
using System.IO.Compression;
using System.Security.Claims;
using System.Text;

namespace StudentPortfolio.Areas.Staff.Pages
{
    [Authorize(Roles = "Staff")]
    public class ExportModel : PageModel
    {
        private readonly StudentPortfolio.Data.ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ExportModel(StudentPortfolio.Data.ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public class StatisticsModel
        {
            public int GoalsCompleted { get; set; }
            public int NumCompetencies { get; set; }
            public int Emerging { get; set; }
            public int Developing { get; set; }
            public int Proficient { get; set; }
            public int Confident { get; set; }
        }

        public IList<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
        public StatisticsModel Statistics = new StatisticsModel();

        public async Task OnGetAsync()
        {
            var users = await _userManager.GetUsersInRoleAsync("Student");
            Users = users.OrderBy(u => u.LastName).ToList();
        }

        public async Task<IActionResult> OnPostExportIndividualStudentDataAsync(string studentId)
        {
            var user = await _context.Users.FindAsync(studentId);

            if (user == null)
            {
                return NotFound();
            }

            var exportData = await CreateExportDataAsync(user);

            return CreateZipFile(exportData, user);
        }

        public async Task<IActionResult> OnPostExportStudentDataOneCSVAsync()
        {
            var users = await _userManager.GetUsersInRoleAsync("Student");
            var date = DateTime.Now;
            var dateString = date.ToString("dd-MM-yyyy");
            var exportData = new StringBuilder();

            foreach (var student in users)
            {
                var summarySb = GetSummaryCsv(student);
                var pitchSb = GetPitchCsv(student);
                var competenciesSb = await GetCompetenciesCsvAsync(student.Id);
                var goalsSb = await GetGoalsCsvAsync(student.Id);
                var CDPSb = await GetCDPCsvAsync(student.Id);
                var statsSb = await GetStatisticsCsvAsync(student.Id);

                exportData
                    .AppendLine($"{student.FirstName} {student.LastName}")
                    .Append(summarySb)
                    .Append(pitchSb)
                    .Append(competenciesSb)
                    .Append(goalsSb)
                    .Append(CDPSb)
                    .Append(statsSb)
                    .AppendLine();
            }

            var csvBytes = Encoding.UTF8.GetBytes(exportData.ToString());

            var fileName = $"All_Student_Data_{dateString}.csv";
            return File(csvBytes, "text/csv", fileName);
        }

        public async Task<IActionResult> OnPostExportAllStudentDataAsync()
        {
            var users = await _userManager.GetUsersInRoleAsync("Student");
            var date = DateTime.Now;
            var dateString = date.ToString("dd-MM-yyyy");

            // Data contains the byte array that CreateZipFile will return
            var studentZips = new List<(byte[] Data, string FileName)>();

            foreach (var student in users)
            {
                // get the individual student's data ready to be converted to a file
                var exportData = await CreateExportDataAsync(student);

                // convert data into byte array (FileContentResult is an object that can store the file
                // as a byte array as well as holding some other important information like the file name)
                var fileResult = CreateZipFile(exportData, student) as FileContentResult;
                if (fileResult != null)
                {
                    studentZips.Add((fileResult.FileContents, fileResult.FileDownloadName));
                }
            }

            // memory stream again to hold the information in memory while the file is being built
            using (var finalZipStream = new MemoryStream())
            {
                // put the memory stream in a ZipArchive which allows us to create a zip file
                using (var zipArchive = new ZipArchive(finalZipStream, ZipArchiveMode.Create, true))
                {
                    foreach (var (data, fileName) in studentZips)
                    {
                        // create new entry (file) inside the final zip with correct file name
                        var entry = zipArchive.CreateEntry(fileName);

                        // open the file, write to the file
                        using (var entryStream = entry.Open())
                        using (var ms = new MemoryStream(data))
                        {
                            await ms.CopyToAsync(entryStream);
                        }
                    }
                }

                // rewind the stream to the beginning so that next time it reads the bytes from the start
                finalZipStream.Position = 0;

                // convert the finalZipStream into a byte array which can then be downloaded by the browser
                return File(finalZipStream.ToArray(), "application/zip", $"All_Student_Portfolios_{dateString}.zip");
            }
        }

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

        private StringBuilder GetSummaryCsv(ApplicationUser user)
        {
            var sb = new StringBuilder();
            var summary = user.Introduction ?? "No personal summary found.";

            sb.AppendLine("Personal Summary");
            sb.AppendLine(CleanCSV(summary));
            sb.AppendLine();
            return sb;
        }

        private StringBuilder GetPitchCsv(ApplicationUser user)
        {
            var sb = new StringBuilder();
            var pitch = user.Pitch ?? "No elevator pitch found.";

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

        private async Task<StringBuilder> GetStatisticsCsvAsync(string userId)
        {
            var sbStats = new StringBuilder();

            int goalsCompletedCount = await _context.Goals
                    .Where(g => g.UserId == userId)
                    .Where(g => g.CompleteDate.HasValue)
                    .CountAsync();

            var competencies = await _context.CompetencyTrackers
                .Where(i => i.UserId == userId)
                .ToListAsync();

            var distinctCompetencyLevels = competencies
                .GroupBy(c => c.CompetencyId)
                .Select(g => new
                {
                    CompetencyId = g.Key,
                    LevelId = g.Max(c => c.LevelId)
                })
                .ToList();

            Statistics = new StatisticsModel
            {
                GoalsCompleted = goalsCompletedCount,
                NumCompetencies = competencies.Count(),
                Emerging = distinctCompetencyLevels.Count(d => d.LevelId == 1),
                Developing = distinctCompetencyLevels.Count(d => d.LevelId == 2),
                Proficient = distinctCompetencyLevels.Count(d => d.LevelId == 3),
                Confident = distinctCompetencyLevels.Count(d => d.LevelId == 4)
            };

            sbStats.AppendLine("\"Goals Completed\",\"Number of Competency Entries\",\"Competencies at Emerging\",\"Competencies at Developing\",\"Competencies at Proficient\",\"Competencies at Confident\"");
            sbStats.AppendLine(
                    $"{CleanCSV(Statistics.GoalsCompleted.ToString())}," +
                    $"{CleanCSV(Statistics.NumCompetencies.ToString())}," +
                    $"{CleanCSV(Statistics.Emerging.ToString())}," +
                    $"{CleanCSV(Statistics.Developing.ToString())}," +
                    $"{CleanCSV(Statistics.Proficient.ToString())}," +
                    $"{CleanCSV(Statistics.Confident.ToString())},"
                );

            return sbStats;
        }

        private async Task<Dictionary<string, StringBuilder>> CreateExportDataAsync(ApplicationUser user)
        {
            var exportData = new Dictionary<string, StringBuilder>();

            // get the date and user name for the file name
            var userName = user.FirstName + "_" + user.LastName;
            var date = DateTime.Now;
            var dateString = date.ToString("dd-MM-yyyy");

            var competenciesSb = await GetCompetenciesCsvAsync(user.Id);
            var goalsSb = await GetGoalsCsvAsync(user.Id);
            var CDPSb = await GetCDPCsvAsync(user.Id);
            var statsSb = await GetStatisticsCsvAsync(user.Id);

            exportData.Add($"{userName}_Personal_Summary_{dateString}.csv", GetSummaryCsv(user));
            exportData.Add($"{userName}_Elevator_Pitch_{dateString}.csv", GetPitchCsv(user));
            exportData.Add($"{userName}_Competencies_{dateString}.csv", competenciesSb);
            exportData.Add($"{userName}_Goals_{dateString}.csv", goalsSb);
            exportData.Add($"{userName}_CDP_{dateString}.csv", CDPSb);
            exportData.Add($"{userName}_General_Statistics_{dateString}.csv", statsSb);

            return exportData;
        }

        private IActionResult CreateZipFile(Dictionary<string, StringBuilder> exportData, ApplicationUser user)
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
                var userName = user.FirstName + "_" + user.LastName;
                var date = DateTime.Now;
                var dateString = date.ToString("dd-MM-yyyy");
                var finalFileName = $"{userName}_Portfolio_Export_{dateString}.zip";

                // export the zip file
                return File(memoryStream.ToArray(), "application/zip", finalFileName);
            }
        }
    }
}
