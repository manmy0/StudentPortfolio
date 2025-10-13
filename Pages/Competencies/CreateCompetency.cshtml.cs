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

        [BindProperty(Name = "competencyId", SupportsGet = true)]
        public int Id { get; set; }

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

            // If the user has no tracker entries then they can only add an entry for the first level
            // If the user has tracker entries then they can add and entry at any level up to 1 above their current highest
            if (!competencyTracker.Any())
            {
                PossibleLevels = await _context.Levels
                    .OrderBy(l => l.Rank)
                    .Take(1)
                    .ToListAsync();
            }
            else
            {
                var allLevels = await _context.Levels
                    .OrderBy(l => l.Rank)
                    .ToListAsync();

                var highestCurrentLevel = competencyTracker
                    .Select(m => m.Level.Rank)
                    .Order()
                    .Take(1);

                var highestPossibleLevel = highestCurrentLevel.First() + 1;
                
                PossibleLevels = allLevels
                    .Where(l => l.Rank == highestPossibleLevel)
                    .ToList();

            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync([Bind("LevelId, StartDate, EndDate, SkillsReview, Evidence")] CompetencyTracker CompetencyTracker)
        {
            
            if (!ModelState.IsValid)
            {
                return Page();

            }

            var competency = await _context.Competencies.FirstOrDefaultAsync(m => m.CompetencyId == Id);
            if (competency == null)
            {
                return NotFound();
            }
            Competency = competency;
            

            CompetencyTracker compTracker = new CompetencyTracker();

            compTracker.CompetencyId = Competency.CompetencyId;
            compTracker.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            compTracker.LevelId = CompetencyTracker.LevelId;
            compTracker.StartDate = CompetencyTracker.StartDate;
            compTracker.EndDate = CompetencyTracker.EndDate;
            compTracker.SkillsReview = CompetencyTracker.SkillsReview;
            compTracker.Evidence = CompetencyTracker.Evidence;
            compTracker.Created = DateTime.Now;
            compTracker.LastUpdated = DateTime.Now;


            _context.CompetencyTrackers.Add(compTracker);
            await _context.SaveChangesAsync();
            return RedirectToPage("Competencies");

        }

    }
}
