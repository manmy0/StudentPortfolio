using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentPortfolio.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace StudentPortfolio.Areas.Admin.Pages.Students
{
    [Authorize(Roles = "Admin")]
    public class StudentsModel : PageModel
    {
        private readonly StudentPortfolio.Data.ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public StudentsModel(StudentPortfolio.Data.ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;

        }

        public IList<ApplicationUser> Users { get; set; }
        public string SearchString { get; set; }

        [BindProperty]
        public string[] SelectedStudentIds { get; set; }

        public async Task OnGetAsync(string searchString)
        {
            SearchString = searchString;

            var usersInRole = await _userManager.GetUsersInRoleAsync("Student");

            // Convert usersInRole variable to an IEnumerable for filtering
            IEnumerable<ApplicationUser> query = usersInRole;

            if (!string.IsNullOrEmpty(searchString))
            {
                // Normalize the search string into lowercase and remove leading spaces
                var search = searchString.Trim().ToLower();

                // Filter users whose FirstName or LastName contains the search string
                query = query.Where(s =>
                  
                    // Checks if the search matches the first name
                   (s.FirstName != null && s.FirstName.ToLower().Contains(search)) ||
                  
                   // Checks if the search matches the last name
                   (s.LastName != null && s.LastName.ToLower().Contains(search)) ||
                    
                   // Checks if the search matches the combined full name 
                   (s.FirstName != null && s.LastName != null && (s.FirstName + " " + s.LastName).ToLower().Contains(search))
            );

            }

            Users = query.ToList();
        }

        public async Task<IActionResult> OnPostDeleteAsync()
        {
            if (SelectedStudentIds != null && SelectedStudentIds.Any())
            {
                foreach (var id in SelectedStudentIds)
                {
                    var user = await _userManager.FindByIdAsync(id);
                    if (user != null)
                    {
                        var result = await _userManager.DeleteAsync(user);
                        if (!result.Succeeded)
                        {
                            ModelState.AddModelError("", $"Error deleting user {user.UserName}: {result.Errors.FirstOrDefault()?.Description}");
                        }
                    }
                }
            }

            return RedirectToPage(new { searchString = SearchString });
        }
    }

}
