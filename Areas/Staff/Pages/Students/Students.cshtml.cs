using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentPortfolio.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace StudentPortfolio.Areas.Staff.Pages.Students
{
    [Authorize(Roles = "Staff")]
    public class StudentsModel : PageModel
    {
        private readonly Data.ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public StudentsModel(Data.ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;

        }

        public IList<ApplicationUser> Users { get; set; }
        public string SearchString { get; set; }

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

                // Filter users whose FirstName OR LastName contains the search string
                query = query.Where(s =>
                    s.FirstName != null && s.FirstName.ToLower().Contains(search) ||
                    s.LastName != null && s.LastName.ToLower().Contains(search)
                );
            }

            Users = query.ToList();
        }


    }
}
