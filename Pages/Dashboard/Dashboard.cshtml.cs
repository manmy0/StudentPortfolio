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

        }

        public ApplicationUser CurrentUser { get; set; }
        public List<CompetencyPerformanceSummaryModel> CompetencyPerformanceSummary { get; set; } = new List<CompetencyPerformanceSummaryModel>();

        public async Task OnGetAsync()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId != null)
            {
                CompetencyPerformanceSummary = await _context.CompetencyTrackers
                    .Where(i => i.UserId == userId)
                    .Include(i => i.Competency) // need the competency table for the display id

                     // groups by the competencyId and displayId
                     // the group will have a key of the competencyId/displayId and values of many competency trackers with the same IDs
                    .GroupBy(i => new { i.CompetencyId, i.Competency.CompetencyDisplayId })
                    .Select(g => new CompetencyPerformanceSummaryModel
                    {
                        CompetencyDisplayId = g.Key.CompetencyDisplayId,
                        TrackerCount = g.Count(),
                        HighestLevelId = g.Max(i => i.LevelId)
                    })
                    .ToListAsync();
            }
        }
    }
}
