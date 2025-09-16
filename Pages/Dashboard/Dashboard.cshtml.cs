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
        public IList<CompetencyTracker> CompetencyTracker { get; set; } = default!;
        public IList<Competency> Competencies { get; set; } = default!;

        // dictionary to store the count of how many entries there are for a specific competency
        public IDictionary<long, int> CompetencyCounts { get; set; } = new Dictionary<long, int>();

        public async Task OnGetAsync()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId != null)
            {
                CompetencyTracker = await _context.CompetencyTrackers
                     .Where(i => i.UserId == userId)
                     .Include(i => i.User)
                     .Include(i => i.Competency)
                     .ToListAsync();

                // find the competencyIds that the user has entries for
                var compIds = await _context.CompetencyTrackers
                                      .Where(i => i.UserId == userId)
                                      .Select(i => i.CompetencyId)
                                      .ToListAsync();

                // get all competencies that the user has a competency tracker for
                Competencies = await _context.Competencies
                    .Where(i => compIds.Contains(i.CompetencyId))
                    .ToListAsync();

                // populate the dictionary that has the id of the competency as the key
                // and the number of times it occurs as the value
                CompetencyCounts = await _context.CompetencyTrackers
                    .Where(i => i.UserId == userId)
                    .GroupBy(i => i.CompetencyId)
                    .ToDictionaryAsync(g => g.Key, g => g.Count());
            }
        }
    }
}

