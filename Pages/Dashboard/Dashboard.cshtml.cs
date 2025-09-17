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

        public ApplicationUser CurrentUser { get; set; }
        public IList<CompetencyTracker> CompetencyTracker { get; set; } = new List<CompetencyTracker>();
        public IList<Competency> Competencies { get; set; } = new List<Competency>();
        public IDictionary<long, int> CompetencyCounts { get; set; } = new Dictionary<long, int>();
        public IList<CompetencyTracker> GroupedTrackers { get; set; } = new List<CompetencyTracker>();
        public IList<CompetencyTracker> LowestCompetencies { get; set; } = new List<CompetencyTracker>();
        public IList<CompetencyTrackerDto> GroupedTrackersDto { get; set; } = new List<CompetencyTrackerDto>();

        public async Task OnGetAsync()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId != null)
            {
                // grab competency trackers for the user logged in
                // puts it in CompetencyTracker (list) for razor page
                CompetencyTracker = await _context.CompetencyTrackers
                     .Where(i => i.UserId == userId)
                     .Include(i => i.User)
                     .Include(i => i.Competency)
                     .ToListAsync();

                // grabs all the competencyIds that the user has entries for
                var compIds = CompetencyTracker
                    .Select(i => i.CompetencyId)
                    .Distinct()
                    .ToList();

                // grab the actual competencies that the user has trackers for from the db
                Competencies = await _context.Competencies
                    .Where(i => compIds.Contains(i.CompetencyId))
                    .ToListAsync();

                // count how many times each competency has been tracked
                CompetencyCounts = await _context.CompetencyTrackers
                    .Where(i => i.UserId == userId)
                    .GroupBy(i => i.CompetencyId)
                    .ToDictionaryAsync(g => g.Key, g => g.Count());

                var levelOrder = new Dictionary<string, int>
                {
                    {"Developing", 1},
                    {"Emerging", 2},
                    {"Capable", 3},
                    {"Competent", 4}
                };

                // pick the highest level for each competency
                // store in list
                GroupedTrackers = CompetencyTracker
                    .GroupBy(t => t.CompetencyId)
                    .Select(group => group.OrderByDescending(t => levelOrder.GetValueOrDefault(t.Level, 0)).First())
                    .ToList();

                LowestCompetencies = GroupedTrackers
                    .OrderBy(t => levelOrder.GetValueOrDefault(t.Level, 5))
                    .Take(5)
                    .ToList();

                GroupedTrackersDto = GroupedTrackers
                    .Select(t => new CompetencyTrackerDto { Level = t.Level })
                    .ToList();
            }
        }

        public class CompetencyTrackerDto
        {
            public string Level { get; set; }
        }
    }
}
