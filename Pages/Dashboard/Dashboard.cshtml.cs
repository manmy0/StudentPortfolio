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

        // grab the selectedYear=202x from the url and assign it to startYear in this controller
        [BindProperty(SupportsGet = true)]
        public int fromYear { get; set; }

        [BindProperty(SupportsGet = true)]
        public int toYear { get; set; }
        public ApplicationUser CurrentUser { get; set; }
        public List<CompetencyPerformanceSummaryModel> CompetencyPerformanceSummary { get; set; } = new List<CompetencyPerformanceSummaryModel>();
        public List<CompetencyPerformanceSummaryModel> LowestFiveCompetencies { get; set; } = new List<CompetencyPerformanceSummaryModel>(5);
        public List<int> AvailableYears { get; set; } = new List<int>();

        public async Task OnGetAsync()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId != null)
            {
                DateOnly currentDate = DateOnly.FromDateTime(DateTime.Now);

                AvailableYears = await _context.CompetencyTrackers
                    .Include(i => i.Competency)
                    .Where(i => i.UserId == userId)
                    .Where(i => i.Competency.EndDate > currentDate || i.Competency.EndDate == null)
                    .Select(i => i.Created.Year)
                    .Distinct()
                    .OrderBy(year => year)
                    .ToListAsync();

                // set to current year if no data is found
                if (!AvailableYears.Any())
                {
                    AvailableYears.Add(DateTime.Now.Year);
                }

                // set from year to the highest available year if it = 0 for some reason
                // set to current year if availableyears doesnt have anything in it
                if (fromYear == 0)
                {
                    fromYear = AvailableYears.Any() ? AvailableYears.Max() : DateTime.Now.Year;
                }

                // set to year to from year if no value given
                if (toYear == 0)
                {
                    toYear = fromYear;
                }

                // some validation checks to make sure it doesnt die in rare circumstances
                if (AvailableYears.Any())
                {
                    if (fromYear < AvailableYears.Min() || fromYear > AvailableYears.Max())
                    {
                        fromYear = AvailableYears.Max();
                    }
                    
                    if (toYear < AvailableYears.Min() || toYear > AvailableYears.Max())
                    {
                        toYear = AvailableYears.Max();
                    }
                }

                var allCompetencyData = await _context.CompetencyTrackers
                    .Include(i => i.Competency) // need the competency table for the display id
                    .Where(i => i.UserId == userId)
                    .Where(i => i.Created.Year <= toYear)
                    .Where(i => i.Created.Year >= fromYear)
                    .Where(i => i.Competency.EndDate > currentDate || i.Competency.EndDate == null)
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
