using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentPortfolio.Models;
using Microsoft.AspNetCore.Http;

namespace StudentPortfolio.Areas.Admin.Pages
{
    [Authorize(Roles = "Admin")]
    public class EditAdminModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public EditAdminModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        // Gets the username from the URL
        [BindProperty(Name = "userName", SupportsGet = true)]
        public string UserName { get; set; }

        [BindProperty]
        public AdminInputModel Input { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.FindByNameAsync(UserName);

            if (user == null)
            {
                return NotFound($"Unable to load user");
            }

            // Adds the user's information to the AdminInputModel
            Input = new AdminInputModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email

            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.FindByNameAsync(UserName);

            if (user == null)
            {
                return NotFound($"Unable to load user");
            }

            // Sets each of the user values to the values in the form
            user.FirstName = Input.FirstName;
            user.LastName = Input.LastName;
            user.Email = Input.Email;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }

            return RedirectToPage("/Admins");
        }
    }

    // AdminInputModel is only used in this class, so it is here
    public class AdminInputModel
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
   
    }
}