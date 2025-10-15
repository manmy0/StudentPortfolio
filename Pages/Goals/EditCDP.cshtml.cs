using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentPortfolio.Models;
using System.Security.Claims;

namespace StudentPortfolio.Pages.Goals
{
    public class EditCDPModel : PageModel
    {
        private readonly StudentPortfolio.Data.ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;


        public EditCDPModel(StudentPortfolio.Data.ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public CareerDevelopmentPlan CDP { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var cdp = await _context.CareerDevelopmentPlans
                .OrderBy(c => c.Year)
                .LastOrDefaultAsync(c => c.UserId == userId);

            if (cdp == null)
            {
                return NotFound();
            }

            CDP = cdp;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            CDP.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(CDP).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CDPExists(CDP.Year, CDP.UserId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("Goals");
        }

        private bool CDPExists(long year, string userId)
        {
            return _context.CareerDevelopmentPlans
                .Any(e => e.Year == year && e.UserId == userId);
        }
    }
}
