using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using StudentPortfolio.Data;
using StudentPortfolio.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace StudentPortfolio.Pages.Goals
{
    public class CreateModel : PageModel
    {
        private readonly StudentPortfolio.Data.ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CreateModel(StudentPortfolio.Data.ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public Goal Goal { get; set; }

        public async Task<IActionResult> OnPostAsync([Bind("Description, Timeline, Startdate, Enddate")] Goal Goal)
        {
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (userId != null)
                {
                    Goal.UserId = userId;

                }

                _context.Goals.Add(Goal);
                await _context.SaveChangesAsync();
                return RedirectToPage("Goals");
            }
        }
    }
}
