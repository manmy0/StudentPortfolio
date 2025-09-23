using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentPortfolio.Models;
using System.Security.Claims;

namespace StudentPortfolio.Pages.Competencies
{
    public class EditCompetencyModel : PageModel
    {
        private readonly StudentPortfolio.Data.ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;


        public EditCompetencyModel(StudentPortfolio.Data.ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public CompetencyTracker CompetencyTracker { get; set; } = default!;
        public Competency Competency { get; set; } = default!;
        public Competency ParentCompetency { get; set; } = default!;

        public string DisplayCompId { get; set; } = string.Empty;

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


        public async Task<IActionResult> OnPostAsync()
        {
            CompetencyTracker.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(CompetencyTracker).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CompetencyTrackerExists(CompetencyTracker))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("Goals");
        }

        private bool CompetencyTrackerExists(CompetencyTracker compTracker)
        {
            return _context.CompetencyTrackers
                .Where(m => m.CompetencyId == compTracker.CompetencyId)
                .Where(m => m.UserId == compTracker.UserId)
                .Where(m => m.Level == compTracker.Level)
                .Any();
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
