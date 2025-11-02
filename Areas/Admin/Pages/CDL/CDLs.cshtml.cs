using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentPortfolio.Models;

namespace StudentPortfolio.Areas.Admin.Pages.CDL
{
    [Authorize]
    public class CDLsModel : PageModel
    {
        private readonly StudentPortfolio.Data.ApplicationDbContext _context;

        public CDLsModel(StudentPortfolio.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Cdl> Cdls { get; set; } = default!;

        public async Task OnGetAsync()
        {
            Cdls = await _context.Cdls.ToListAsync();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var cdlToDelete = await _context.Cdls
                .FirstOrDefaultAsync(g => g.CdlId == id);

            if (cdlToDelete == null)
            {
                return NotFound();
            }

            _context.Cdls.Remove(cdlToDelete);
            await _context.SaveChangesAsync();

            return RedirectToPage("CDLs");
        }
    }
}
