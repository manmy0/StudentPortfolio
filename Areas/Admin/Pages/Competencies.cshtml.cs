using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentPortfolio.Models;

namespace StudentPortfolio.Areas.Admin.Pages
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
                    .ToListAsync();

            ParentCompetencies = await _context.Competencies
                .Where(i => i.ParentCompetencyId == null)
                .ToListAsync();
        }
    }
}
