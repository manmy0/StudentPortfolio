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

        public async Task<IActionResult> OnGetAsync(CompetencyTracker? compTracker)
        {
            if (compTracker == null)
            {
                return NotFound();
            }

            var competencyTracker = await _context.CompetencyTrackers
                .Where(m => m.CompetencyId == compTracker.CompetencyId)
                .Where(m => m.UserId == compTracker.UserId)
                .Where(m => m.Level == compTracker.Level)
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

        public String GetDisplayCompId(long compId)
        {
            var competency = _context.Competencies.FirstOrDefault(m => m.CompetencyId == compId);

            if (competency == null)
            {
                return null;
            }

            if (competency.ParentCompetencyId == null)
            {
                return competency.CompetencyId.ToString();
            }

            return competency.ParentCompetencyId.ToString() + ". " + competency.CompetencyId.ToString() + ".";
        }
    }
}
