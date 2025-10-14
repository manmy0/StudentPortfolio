using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentPortfolio.Models;
using System.Security.Claims;

namespace StudentPortfolio.Pages.Dashboard
{
    [Authorize]
    public class DashboardModel : PageModel
    {
        private readonly StudentPortfolio.Data.ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardModel(StudentPortfolio.Data.ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // model for the competency performance summary table
        public class CompetencyPerformanceSummaryModel
        {
            public string CompetencyDisplayId { get; set; }
            public int TrackerCount { get; set; }
            public long HighestLevelId { get; set; }
            public string Description { get; set; }

        }

        public ApplicationUser CurrentUser { get; set; }
        public List<CompetencyPerformanceSummaryModel> CompetencyPerformanceSummary { get; set; } = new List<CompetencyPerformanceSummaryModel>();
        public List<CompetencyPerformanceSummaryModel> LowestFiveCompetencies { get; set; } = new List<CompetencyPerformanceSummaryModel>(5);

        public async Task OnGetAsync(int selectedYear)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId != null)
            {
                if (selectedYear == 0)
                {
                    // default to current year if no year is selected
                    selectedYear = DateTime.Now.Year; 
                }

                var allCompetencyData = await _context.CompetencyTrackers
                    .Where(i => i.UserId == userId)
                    .Where(i => i.Created.Year <= selectedYear)
                    .Include(i => i.Competency) // need the competency table for the display id

                    // groups by the competencyId and displayId
                    // the group will have a key of the competencyId/displayId and values of many competency trackers with the same IDs
                    // now also grouping by description because it is one to one so it works
                    .GroupBy(i => new { i.CompetencyId, i.Competency.CompetencyDisplayId, i.Competency.Description })
                    .Select(g => new CompetencyPerformanceSummaryModel
                    {
                        CompetencyDisplayId = g.Key.CompetencyDisplayId,
                        TrackerCount = g.Count(),
                        HighestLevelId = g.Max(i => i.LevelId),
                        Description = g.Key.Description
                    })
                    .ToListAsync();

                CompetencyPerformanceSummary = allCompetencyData;

                // apparently its good to reuse code so just grabbing the same query
                // from before and taking the lowest 5 levels from it
                LowestFiveCompetencies = allCompetencyData
                    .OrderBy(c => c.HighestLevelId)
                    .Take(5)
                    .ToList();
            }
        }
    }
}
