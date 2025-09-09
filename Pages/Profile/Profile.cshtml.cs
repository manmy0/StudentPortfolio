using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentPortfolio.Data;
using StudentPortfolio.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Security.Claims;

namespace StudentPortfolio.Pages.Profile
{
    [Authorize]
    public class ProfileModel : PageModel
    {
        private readonly StudentPortfolio.Data.ApplicationDbContext _context; 
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileModel(StudentPortfolio.Data.ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public ApplicationUser CurrentUser { get; set; }
        public IList<CareerDevelopmentPlan> CDP { get; set; } = default!;

        // convert the users byte representation of an image
        // in the database to base64 which can be displayed
        public string? ProfileImageBase64 =>
            CurrentUser?.ProfileImage != null
                ? $"data:image/jpeg;base64,{Convert.ToBase64String(CurrentUser.ProfileImage)}"
                : null;

        public async Task OnGetAsync()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId != null)
            {
                CurrentUser = await _userManager.FindByIdAsync(userId);

                CDP = await _context.CareerDevelopmentPlans
                   .Where(i => i.UserId == userId)
                   .Include(i => i.User)
                   .ToListAsync();
            }

        }
    }
}
