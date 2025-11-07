using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentPortfolio.Models;
using System.Security.Claims;

namespace StudentPortfolio.Areas.Admin.Pages.Competencies
{
    [Authorize(Roles = "Admin")]
    public class CompetenciesModel : PageModel
    {
        private readonly StudentPortfolio.Data.ApplicationDbContext _context;

        public CompetenciesModel(StudentPortfolio.Data.ApplicationDbContext context)
        {
            _context = context;
           
        }

        public IList<Competency> Competencies { get; set; } = default!;

        public IList<Competency> ParentCompetencies { get; set; } = default!;

        public async Task OnGetAsync()
        {
            Competencies = await _context.Competencies
                .Include(i => i.CompetencyTrackers)
                .OrderBy(i => i.CompetencyDisplayId)
                .ToListAsync();

            ParentCompetencies = await _context.Competencies
                .Where(i => i.ParentCompetencyId == null)
                .OrderBy(i => i.CompetencyDisplayId)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            
            var competencyToDelete = await _context.Competencies
                .FirstOrDefaultAsync(g => g.CompetencyId == id);

            if (competencyToDelete == null)
            {
                return NotFound();
            }

            _context.Competencies.Remove(competencyToDelete);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Competencies");
        }
    }
}
