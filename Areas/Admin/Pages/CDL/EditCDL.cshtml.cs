using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentPortfolio.Models;

namespace StudentPortfolio.Areas.Admin.Pages.CDL
{
    public class EditCDLModel : PageModel
    {
        private readonly StudentPortfolio.Data.ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public EditCDLModel(StudentPortfolio.Data.ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public Cdl Cdl { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(long? cdlId)
        {
            if (cdlId == null)
            {
                return NotFound();
            }

            var cdl = await _context.Cdls
                .FirstAsync(i => i.CdlId == cdlId);

            if (cdl == null)
            {
                return NotFound();
            }

            Cdl = cdl;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Cdl.LastUpdated = DateTime.Now;

            //_context.Attach(Competency).State = EntityState.Modified;

            _context.Cdls.Update(Cdl);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CdlExists(Cdl.CdlId))
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
            return RedirectToPage("./CDLs");
        }

        private bool CdlExists(long id)
        {
            return _context.Cdls.Any(e => e.CdlId == id);
        }
    }
}
