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

        public List<SelectListItem> MyDropdownItems { get; set; }
        public List<CompetencyTracker> MyDropdown { get; set; }
        public async Task OnGetAsync()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId != null)
            {
                CompetencyTracker = await _context.CompetencyTrackers
                     .Where(i => i.UserId == userId)
                     .Include(i => i.User)
                     .OrderBy(i => i.CompetencyId)
                     .ThenByDescending(i => i.Created)
                     .ToListAsync();

                var compIds = await _context.CompetencyTrackers
                                      .Where(i => i.UserId == userId)
                                      .Select(i => i.CompetencyId)
                                      .ToListAsync();
                
                Competencies = await _context.Competencies
                    .Where(i => compIds.Contains(i.CompetencyId))
                    .ToListAsync();

                ParentCompetencies = await _context.Competencies
                    .Where(i => i.ParentCompetencyId == null)
                    .ToListAsync();
            }
        }

        /*public void OnGet()
        {

            MyDropdownItems = new List<SelectListItem>
        {
            new SelectListItem { Text = "Option A", Value = "A", },
            new SelectListItem { Text = "Option B", Value = "B", Selected = true }, // Pre-select an item
            new SelectListItem { Text = "Option C", Value = "C" }
        };
            MyDropdown = new List<CompetencyTracker>
            {

            }
        }*/
    }
}
