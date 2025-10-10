using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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

        public CompetencyTracker CompetencyTracker { get; set; } = default!;
        public Competency Competency { get; set; } = default!;
        public Competency ParentCompetency { get; set; } = default!;

        public IList<Level> PossibleLevels { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(long? competencyId)
        {

            var competency = await _context.Competencies.FirstOrDefaultAsync(m => m.CompetencyId == competencyId);

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


            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var competencyTracker = await _context.CompetencyTrackers
                .Include(m => m.Level)
                .Where(m => m.CompetencyId == competencyId && m.UserId == userId)
                .ToListAsync();

            if (!competencyTracker.Any())
            {
                PossibleLevels = await _context.Levels
                    .OrderBy(l => l.Rank)
                    .Take(1)
                    .ToListAsync();
            }
            else
            {
                PossibleLevels = await _context.Levels
                    .OrderBy(l => l.Rank)
                    .ToListAsync();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync([Bind("CompetencyId, UserId, LevelId, StartDate, EndDate, SkillsReview, Evidence")] CompetencyTracker CompetencyTracker)
        {
            
            if (!ModelState.IsValid)
            {
                return Page();

            }

            PossibleLevels = await _context.Levels
                    .OrderBy(l => l.Rank)
                    .ToListAsync();

            CompetencyTracker.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            _context.CompetencyTrackers.Add(CompetencyTracker);
            await _context.SaveChangesAsync();
            return RedirectToPage("Competencies");

        }

    }
}
