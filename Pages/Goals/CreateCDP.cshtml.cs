using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentPortfolio.Models;
using System.Security.Claims;

namespace StudentPortfolio.Pages.Goals
{
    public class CreateCDPModel : PageModel
    {
        private readonly StudentPortfolio.Data.ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CreateCDPModel(StudentPortfolio.Data.ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        
        public CareerDevelopmentPlan NewCDP { get; set; } = default!;

        public CareerDevelopmentPlan LatestCDP { get; set; } = default!;


        public async Task<IActionResult> OnGetAsync()
        {

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var latestcdp = await _context.CareerDevelopmentPlans
                .OrderBy(c => c.Year)
                .LastOrDefaultAsync(c => c.UserId == userId);

            //if (latestcdp == null)
            //{
            //    return NotFound();
            //}

            LatestCDP = latestcdp;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync([Bind("ProfessionalInterests, EmployersOfInterest, PersonalValues, DevelopmentFocus, Extracurricular, NetworkingPlan")] CareerDevelopmentPlan NewCDP)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            NewCDP.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            NewCDP.Year = (short)DateTime.Now.Year;

            //_context.Attach(NewCDP).State = EntityState.Modified;

            _context.CareerDevelopmentPlans.Add(NewCDP);
            await _context.SaveChangesAsync();
            return RedirectToPage("Goals");

            //try
            //{
            //    await _context.SaveChangesAsync();
            //}
            //catch (DbUpdateConcurrencyException)
            //{
            //    if (!CDPExists(NewCDP.Year, NewCDP.UserId))
            //    {
            //        return NotFound();
            //    }
            //    else
            //    {
            //        throw;
            //    }
            //}

            
        }

        private bool CDPExists(long year, string userId)
        {
            return _context.CareerDevelopmentPlans
                .Any(e => e.Year == year && e.UserId == userId);
        }
    }
}
