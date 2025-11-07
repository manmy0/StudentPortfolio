using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentPortfolio.Models;

namespace StudentPortfolio.Areas.Admin.Pages.Staff
{
    [Authorize(Roles = "Admin")]
    public class StaffModel : PageModel
    {
        private readonly Data.ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public StaffModel(Data.ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;

        }

        public IList<ApplicationUser> Users { get; set; }
        public string SearchString { get; set; }

        [BindProperty]
        public string[] SelectedStaffIds { get; set; }

        public async Task OnGetAsync(string searchString)
        {
            SearchString = searchString;

            var usersInRole = await _userManager.GetUsersInRoleAsync("Staff");

            // Convert usersInRole variable to an IEnumerable for filtering
            IEnumerable<ApplicationUser> query = usersInRole;

            if (!string.IsNullOrEmpty(searchString))
            {
                // Normalize the search string into lowercase and remove leading spaces
                var search = searchString.Trim().ToLower();

                // Filter users whose FirstName OR LastName contains the search string
                query = query.Where(s =>
                    s.FirstName != null && s.FirstName.ToLower().Contains(search) ||
                    s.LastName != null && s.LastName.ToLower().Contains(search)
                );
            }

            Users = query.ToList();
        }


        public async Task<IActionResult> OnPostDeleteAsync()
        {
            if (SelectedStaffIds != null && SelectedStaffIds.Any())
            {
                foreach (var id in SelectedStaffIds)
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


