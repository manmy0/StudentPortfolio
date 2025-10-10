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

        public ApplicationUser CurrentUser { get; set; }
        public IList<CompetencyTracker> CompetencyTracker { get;set; } = default!;
        public IList<Competency> Competencies { get; set; } = default!;

        public IList<Competency> ParentCompetencies { get; set; } = default!;

        public async Task OnGetAsync()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId != null)
            {
                
                CompetencyTracker = await _context.CompetencyTrackers
                     .Where(i => i.UserId == userId)
                     .Include(i => i.User)
                     .Include(i => i.Level)
                     .OrderBy(i => i.CompetencyId)
                     .ThenByDescending(i => i.Level.Rank)
                     .ToListAsync();

                var compIds = await _context.CompetencyTrackers
                                      .Where(i => i.UserId == userId)
                                      .Select(i => i.CompetencyId)
                                      .ToListAsync();
                
                Competencies = await _context.Competencies
                    .ToListAsync();

                ParentCompetencies = await _context.Competencies
                    .Where(i => i.ParentCompetencyId == null)
                    .ToListAsync();
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var goalToDelete = await _context.CompetencyTrackers
                .FirstOrDefaultAsync(g => g.CompetencyTrackerId == id && g.UserId == userId);

            if (goalToDelete == null)
            {
                return NotFound();
            }

            _context.CompetencyTrackers.Remove(goalToDelete);
            await _context.SaveChangesAsync();

            return RedirectToPage("Goals");
        }


    }
}
