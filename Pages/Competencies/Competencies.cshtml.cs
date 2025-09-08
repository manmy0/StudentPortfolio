using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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

        public async Task OnGetAsync()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId != null)
            {
                CompetencyTracker = await _context.CompetencyTrackers
                     .Where(i => i.UserId == userId)
                     .Include(i => i.User)
                     .ToListAsync();

                var compIds = await _context.CompetencyTrackers
                                      .Where(i => i.UserId == userId)
                                      .Select(i => i.CompetencyId)
                                      .ToListAsync();
                
                Competencies = await _context.Competencies
                    .Where(i => compIds.Contains(i.CompetencyId))
                    .ToListAsync();
            }
        }
    }
}
