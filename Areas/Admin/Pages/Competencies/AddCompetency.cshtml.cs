using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentPortfolio.Models;

namespace StudentPortfolio.Areas.Admin.Pages
{
    public class AddCompetencyModel : PageModel
    {
        private readonly StudentPortfolio.Data.ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AddCompetencyModel(StudentPortfolio.Data.ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public Competency Competency { get; set; } = new Competency();

        public IList<Competency> ParentCompetencies { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            ParentCompetencies = await _context.Competencies
                    .Where(i => i.ParentCompetencyId == null)
                    .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Competency.LastUpdated = DateTime.Now;

            _context.Competencies.Add(Competency);
            await _context.SaveChangesAsync();
            return RedirectToPage("/Competencies/Competencies");
        }
    }
}
