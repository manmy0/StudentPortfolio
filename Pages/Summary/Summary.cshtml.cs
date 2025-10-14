using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentPortfolio.Models;
using System.Security.Claims;

namespace StudentPortfolio.Pages.Summary
{
    [Authorize]
    public class SummaryModel : PageModel
    {
        private readonly StudentPortfolio.Data.ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ApplicationUser CurrentUser { get; set; }

        public SummaryModel(StudentPortfolio.Data.ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /* 
         Things i want to get:
            Image, name, degree, specialisation of user
            Goals completed in year
            Number of competencies at each level
            
         */

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
            }

        }
    }
}

