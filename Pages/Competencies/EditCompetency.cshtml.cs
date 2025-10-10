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

        public async Task<IActionResult> OnGetAsync(long? competencyTrackerId)
        {
            if (competencyTrackerId == null)
            {
                return NotFound();
            }

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return NotFound();
            }

            var competencyTracker = await _context.CompetencyTrackers
                .Where(m => m.CompetencyTrackerId == competencyTrackerId)
                .Where(m => m.UserId == userId)
                .Include(m => m.Level)
                .FirstOrDefaultAsync();

            if (competencyTracker == null)
            {
                return NotFound();
            }

            CompetencyTracker = competencyTracker;

            var competency = await _context.Competencies.FirstOrDefaultAsync(m => m.CompetencyId == CompetencyTracker.CompetencyId);

            Competency = competency;

            var parentCompetency = await _context.Competencies.FirstOrDefaultAsync(m => m.CompetencyId == Competency.ParentCompetencyId);

            
            ParentCompetency = parentCompetency;

            return Page();
        }


        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // StartDate must be before EndDate
            if (CompetencyTracker.StartDate >= CompetencyTracker.EndDate)
                {
                    ModelState.AddModelError("CompetencyTracker.EndDate", "End Date must be after Start Date.");
                    return Page();
                }

            var compTracker = await _context.CompetencyTrackers
                .Include(c => c.Level)
                .FirstOrDefaultAsync(m => m.CompetencyTrackerId == CompetencyTracker.CompetencyTrackerId
                           && m.UserId == userId);

            if (compTracker == null)
                return NotFound();

            compTracker.StartDate = CompetencyTracker.StartDate;
            compTracker.EndDate = CompetencyTracker.EndDate;
            compTracker.SkillsReview = CompetencyTracker.SkillsReview;
            compTracker.Evidence = CompetencyTracker.Evidence;
            compTracker.LastUpdated = DateTime.Now;

            //await _context.SaveChangesAsync();

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CompetencyTrackerExists(CompetencyTracker.CompetencyTrackerId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Competencies");
        }

        private bool CompetencyTrackerExists(long compTrackerId)
        {
            return _context.CompetencyTrackers
                .Where(m => m.CompetencyTrackerId == compTrackerId)
                .Any();
        }
    }
}
