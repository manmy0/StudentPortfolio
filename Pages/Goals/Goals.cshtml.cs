using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StudentPortfolio.Data;
using StudentPortfolio.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace StudentPortfolio.Pages.Goals
{
    [Authorize]
    public class GoalsModel : PageModel
    {
        private readonly StudentPortfolio.Data.ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public GoalsModel(StudentPortfolio.Data.ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;

        }

        [BindProperty(SupportsGet = true)]
        public short selectedYear { get; set; }
        public IList<short> PossibleYears { get; set; } = default!;
        public ApplicationUser CurrentUser { get; set; }
        public IList<Goal> Goal { get;set; } = default!;
        public IList<CareerDevelopmentPlan> CDP { get; set; } = default!;
        public IList<GoalStep> GoalSteps { get; set; } = default!;

        public async Task OnGetAsync()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId != null)
            {
                short thisYear = (short)DateTime.Now.Year;

                

                Goal = await _context.Goals
                    .Where(i => i.UserId == userId)
                    .Include(i => i.User)
                    .ToListAsync();

                var goalIds = await _context.Goals
                    .Where(i => i.UserId == userId)
                    .Select(i => i.GoalId)
                    .ToListAsync();

                GoalSteps = await _context.GoalSteps
                    .Where(i => goalIds.Contains(i.GoalId))
                    .ToListAsync();

                var AllCDP = await _context.CareerDevelopmentPlans
                    .Where(i => i.UserId == userId)
                    .Include(i => i.User)
                    .ToListAsync();

                PossibleYears = AllCDP
                    .Select(i => i.Year)
                    .ToList();

                // Show the CDP entry for a specific year if a year has been selected
                if (selectedYear != 0)
                {
                    CDP = AllCDP
                        .Where(i => i.Year == selectedYear)
                        .ToList();
                }
                // Show the most recent CDP entry if the user has any and a year hasn't been selected
                else if (PossibleYears.Count() != 0)
                {
                    CDP = AllCDP
                        .Where(i => i.Year == PossibleYears.Last())
                        .ToList();

                    selectedYear = PossibleYears.Last();
                }
                // Default to showing this years CDP entry even if it's blank
                else
                {
                    CDP = AllCDP
                        .Where(i => i.Year == thisYear)
                        .ToList();

                    selectedYear = thisYear;
                }

                // Even if the user doesn't have an entry for this year, list it as an option
                if (!PossibleYears.Contains(thisYear))
                {
                    PossibleYears.Add(thisYear);
                }
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var goalToDelete = await _context.Goals
                .FirstOrDefaultAsync(g => g.GoalId == id && g.UserId == userId);

            if (goalToDelete == null)
            {
                return NotFound();
            }

            var stepsToDelete = await _context.GoalSteps
                .Where(s => s.GoalId == id)
                .ToListAsync();


            if (stepsToDelete.Any())
            {
                _context.GoalSteps.RemoveRange(stepsToDelete);
            }

            _context.Goals.Remove(goalToDelete);
            await _context.SaveChangesAsync();

            return RedirectToPage("Goals"); 
        }
    }
}
