using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentPortfolio.Models;
using System.Security.Claims;
using static StudentPortfolio.Pages.Dashboard.DashboardModel;

namespace StudentPortfolio.Pages.Summary
{
    [Authorize]
    public class SummaryModel : PageModel
    {
        private readonly StudentPortfolio.Data.ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ApplicationUser CurrentUser { get; set; }

        public SummaryModel(StudentPortfolio.Data.ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public class SummaryViewModel
        {
            public int GoalsCompleted { get; set; }
            public int NumCompetencies { get; set; }
            public int Emerging { get; set; }
            public int Developing { get; set; }
            public int Proficient { get; set; }
            public int Confident { get; set; }
        }

        // grab the selectedYear=202x from the url and assign it to selectedYear in this controller
        [BindProperty(SupportsGet = true)]
        public int selectedYear { get; set; }
        public SummaryViewModel SummaryData = new SummaryViewModel();
        public List<int> AvailableYears { get; set; } = new List<int>();

        public string? ProfileImageBase64 =>
            CurrentUser?.ProfileImage != null
                ? $"data:image/jpeg;base64,{Convert.ToBase64String(CurrentUser.ProfileImage)}"
                : null;

        public async Task OnGetAsync()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId != null)
            {
                CurrentUser = await _userManager.FindByIdAsync(userId);

                AvailableYears = await _context.CompetencyTrackers
                    .Where(i => i.UserId == userId)
                    .Select(i => i.Created.Year)
                    .Distinct()
                    .OrderBy(year => year)
                    .ToListAsync();

                // set to current year if no data is found
                if (!AvailableYears.Any())
                {
                    AvailableYears.Add(DateTime.Now.Year);
                }

                // set year to the highest available year if it = 0 for some reason
                // set year to current year if available years doesnt have anything in it
                if (selectedYear == 0)
                {
                    selectedYear = AvailableYears.Any() ? AvailableYears.Max() : DateTime.Now.Year;
                }

                int goalsCompletedCount = await _context.Goals
                    .Where(g => g.UserId == userId)
                    .Where(g => g.CompleteDate.HasValue && g.CompleteDate.Value.Year == selectedYear)
                    .CountAsync();

                var competencies = await _context.CompetencyTrackers
                    .Where(i => i.UserId == userId)
                    .Where(i => i.Created.Year == selectedYear)
                    .ToListAsync();

                var distinctCompetencyLevels = competencies
                    .GroupBy(c => c.CompetencyId)
                    .Select(g => new
                    {
                        CompetencyId = g.Key,
                        LevelId = g.Max(c => c.LevelId)
                    })
                    .ToList();

                SummaryData = new SummaryViewModel
                {
                    GoalsCompleted = goalsCompletedCount,
                    NumCompetencies = competencies.Count(),
                    Emerging = distinctCompetencyLevels.Count(d => d.LevelId == 1),
                    Developing = distinctCompetencyLevels.Count(d => d.LevelId == 2),
                    Proficient = distinctCompetencyLevels.Count(d => d.LevelId == 3),
                    Confident = distinctCompetencyLevels.Count(d => d.LevelId == 4)
                };
            }

        }
    }
}

