using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentPortfolio.Models;

namespace StudentPortfolio.Areas.Admin.Pages
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
        public Competency Competency { get; set; } = default!;

        public Competency ParentCompetency { get; set; } = default!;

        public IList<Competency> ParentCompetencies { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(long? competencyId)
        {
            if (competencyId == null)
            {
                return NotFound();
            }

            var competency = await _context.Competencies
                .FirstAsync(i => i.CompetencyId == competencyId);

            if (competency == null)
            {
                return NotFound();
            }

            Competency = competency;

            if (Competency.ParentCompetencyId != null)
            {
                ParentCompetency = await _context.Competencies
                    .FirstOrDefaultAsync(i => i.CompetencyId == Competency.ParentCompetencyId);
            }

            ParentCompetencies = await _context.Competencies
                .Where(i => i.ParentCompetencyId == null)
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Competency.LastUpdated = DateTime.Now;

            //_context.Attach(Competency).State = EntityState.Modified;
            
            _context.Competencies.Update(Competency);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CompetencyExists(Competency.CompetencyId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            //_context.Competencies.Add(Competency);
            //await _context.SaveChangesAsync();
            return RedirectToPage("/Competencies");
        }

        private bool CompetencyExists(long id)
        {
            return _context.Competencies.Any(e => e.CompetencyId == id);
        }
    }
}
