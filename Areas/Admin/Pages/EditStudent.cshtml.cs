using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentPortfolio.Models;
using Microsoft.AspNetCore.Http;

namespace StudentPortfolio.Areas.Admin.Pages
{
    [Authorize(Roles = "Admin")]
    public class EditStudentsModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public EditStudentsModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [BindProperty(Name = "userName", SupportsGet = true)]
        public string UserName { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.FindByNameAsync(UserName);

            if (user == null)
            {
                return NotFound($"Unable to load user");
            }

            Input = new InputModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Degree = user.Degree,
                Introduction = user.Introduction,
                Specialisation = user.Specialisation,
                LinkedIn = user.LinkedIn,
                Resume = user.Resume
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

            user.FirstName = Input.FirstName;
            user.LastName = Input.LastName;
            user.Degree = Input.Degree;
            user.Introduction = Input.Introduction;
            user.Specialisation = Input.Specialisation;
            user.LinkedIn = Input.LinkedIn;
            user.Resume = Input.Resume;

            if (Input.ProfileImage != null && Input.ProfileImage.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await Input.ProfileImage.CopyToAsync(memoryStream);

                    user.ProfileImage = memoryStream.ToArray();
                }
            }

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }

            return RedirectToPage("/Students");
        }
    }

    public class InputModel
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Degree { get; set; }
        public string? Introduction { get; set; }
        public string? Specialisation { get; set; }
        public string? LinkedIn { get; set; }
        public string? Resume { get; set; }
        public IFormFile? ProfileImage { get; set; }
    }
}