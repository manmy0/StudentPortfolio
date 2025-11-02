using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentPortfolio.Models;

namespace StudentPortfolio.Areas.Admin.Pages.CDL
{
    public class CreateCDLModel : PageModel
    {
        private readonly StudentPortfolio.Data.ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CreateCDLModel(StudentPortfolio.Data.ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public Cdl Cdl { get; set; } = new Cdl();

        public async Task<IActionResult> OnPostAsync()
        {
            Cdl.DateCreated = DateTime.Now;
            Cdl.LastUpdated = DateTime.Now;

            //_context.Attach(Competency).State = EntityState.Modified;

            //_context.Cdls.Update(Cdl);

            _context.Cdls.Add(Cdl);
            await _context.SaveChangesAsync();
            return RedirectToPage("./CDLs");
        }
    }
}
