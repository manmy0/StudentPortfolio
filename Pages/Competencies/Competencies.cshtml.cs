using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentPortfolio.Data;
using StudentPortfolio.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace StudentPortfolio.Pages.Competencies
{
    [Authorize]
    public class CompetenciesModel : PageModel
    {
        private readonly StudentPortfolio.Data.ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CompetenciesModel(StudentPortfolio.Data.ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty(SupportsGet = true)]
        public string? From { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? To { get; set; }

        public ApplicationUser CurrentUser { get; set; }
        public IList<CompetencyTracker> CompetencyTracker { get;set; } = default!;
        
        public IList<Competency> Competencies { get; set; } = default!;

        public IList<Competency> ParentCompetencies { get; set; } = default!;

        public IList<CompetencyTracker> DiscontinuedTrackers { get; set; } = default!;
        public IList<Competency> DiscontinuedCompetencies { get; set; } = default!;

        public async Task OnGetAsync()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId != null)
            {
                
                var competencyTrackers = await _context.CompetencyTrackers
                     .Where(i => i.UserId == userId)
                     .Include(i => i.User)
                     .Include(i => i.Level)
                     .OrderBy(i => i.CompetencyId)
                     .ThenByDescending(i => i.Level.Rank)
                     .ThenByDescending(i => i.StartDate)
                     .ThenByDescending(i => i.EndDate)
                     .ToListAsync();

                //DiscontinuedTrackers = competencyTrackers
                //    .Where(c => c.Competency.EndDate != null)
                //    .ToList();


                Competencies = await _context.Competencies
                    .Where(i => i.EndDate == null)
                    .OrderBy(i => i.CompetencyDisplayId)
                    .ToListAsync();

                DiscontinuedCompetencies = await _context.Competencies
                    .Where(i => i.EndDate != null)
                    .OrderBy(i => i.CompetencyDisplayId)
                    .ToListAsync();

                ParentCompetencies = await _context.Competencies
                    .Where(i => i.ParentCompetencyId == null)
                    .ToListAsync();

                // If the user provided a date range
                if (!string.IsNullOrEmpty(From) && !string.IsNullOrEmpty(To))
                {
                    // Parse inputs like "2025-03" into full DateOnly
                    DateOnly fromDate = DateOnly.ParseExact(From + "-01", "yyyy-MM-dd", null);
                    // Set 'toDate' as the last day of that month
                    DateOnly toDate = DateOnly.ParseExact(To + "-01", "yyyy-MM-dd", null)
                        .AddMonths(1)
                        .AddDays(-1);

                    // Filter competency trackers that fall within this date range
                    CompetencyTracker = competencyTrackers
                        .Where(g =>
                            (g.StartDate >= fromDate && g.StartDate <= toDate) ||  // start date in range
                            (g.EndDate >= fromDate && g.EndDate <= toDate) ||      // end date in range
                            (g.StartDate <= fromDate && g.EndDate >= toDate)       // tracker spans entire range
                        )
                        .ToList();
                }
                else
                {
                    CompetencyTracker = competencyTrackers;
                }

                DiscontinuedTrackers = CompetencyTracker
                    .Where(c => c.Competency.EndDate != null)
                    .ToList();

            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var competencyToDelete = await _context.CompetencyTrackers
                .FirstOrDefaultAsync(g => g.CompetencyTrackerId == id && g.UserId == userId);

            if (competencyToDelete == null)
            {
                return NotFound();
            }

            _context.CompetencyTrackers.Remove(competencyToDelete);
            await _context.SaveChangesAsync();

            return RedirectToPage("Competencies");
        }

    }
}
