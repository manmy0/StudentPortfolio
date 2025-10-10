using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentPortfolio.Models;
using System.Security.Claims;

namespace StudentPortfolio.Pages.Competencies
{
    [Authorize]
    public class CreateCompetencyModel : PageModel
    {
        private readonly StudentPortfolio.Data.ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CreateCompetencyModel(StudentPortfolio.Data.ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public CompetencyTracker CompetencyTracker { get; set; }
        public Competency Competency { get; set; }
        public Competency ParentCompetency { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return NotFound();
            }

            var competencyTracker = await _context.CompetencyTrackers
                .Where(m => m.CompetencyTrackerId == id)
                .Where(m => m.UserId == userId)
                .Include(m => m.Level)
                .FirstOrDefaultAsync();

            if (competencyTracker == null)
            {
                return NotFound();
            }

            CompetencyTracker = competencyTracker;

            var competency = await _context.Competencies.FirstOrDefaultAsync(m => m.CompetencyId == CompetencyTracker.CompetencyId);

            if (competency == null)
            {
                return NotFound();
            }

            Competency = competency;

            var parentCompetency = await _context.Competencies.FirstOrDefaultAsync(m => m.CompetencyId == Competency.ParentCompetencyId);

            if (parentCompetency == null)
            {
                return NotFound();
            }

            ParentCompetency = parentCompetency;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync([Bind("StartDate, EndDate, Level, SkillsReview, Evidence")] CompetencyTracker CompetencyTracker)
        {
            CompetencyTracker.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!ModelState.IsValid)
            {
                return Page();

            }

            _context.CompetencyTrackers.Add(CompetencyTracker);
            await _context.SaveChangesAsync();
            return RedirectToPage("Competencies");

        }

    }
}
