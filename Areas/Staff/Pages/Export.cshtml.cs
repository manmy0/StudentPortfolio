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

        public IList<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();

        public async Task OnGetAsync()
        {
            Users = await _userManager.GetUsersInRoleAsync("Student");
        }

        public async Task<IActionResult> OnPostExportIndividualStudentDataAsync(string studentId)
        {
            var user = await _context.Users.FindAsync(studentId);

            if (user == null)
            {
                return NotFound();
            }

            var exportData = new Dictionary<string, StringBuilder>();

            // get the date and user name for the file name
            var userName = user.FirstName + "_" + user.LastName;
            var date = DateTime.Now;
            var dateString = date.ToString("dd-MM-yyyy");

            exportData.Add($"{userName}_Personal_Summary_{dateString}.csv", GetSummaryCsv(user));
            exportData.Add($"{userName}_Elevator_Pitch_{dateString}.csv", GetPitchCsv(user));

            return CreateZipFile(exportData, user);
        }

        public async Task<IActionResult> OnPostExportAllStudentDataAsync()
        {
            Users = await _userManager.GetUsersInRoleAsync("Student");
            return Page();
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
