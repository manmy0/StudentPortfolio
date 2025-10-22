using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentPortfolio.Models;

namespace StudentPortfolio.Areas.Staff.Pages
{
    [Authorize(Roles = "Staff")]
    public class ExportModel : PageModel
    {
        private readonly StudentPortfolio.Data.ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ExportModel(StudentPortfolio.Data.ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IList<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();

        public async Task OnGetAsync()
        {
            Users = await _userManager.GetUsersInRoleAsync("Student");
        }

        public async Task<IActionResult> OnPostExportIndividualStudentDataAsync(string studentId)
        {
            Users = await _userManager.GetUsersInRoleAsync("Student");

            return Page();
        }

        public async Task<IActionResult> OnPostExportAllStudentDataAsync()
        {
            Users = await _userManager.GetUsersInRoleAsync("Student");
            return Page();
        }
    }
}
