using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentPortfolio.Models;
using Microsoft.AspNetCore.Http;

namespace StudentPortfolio.Pages.Profile
{
    [Authorize]
    public class EditModel : PageModel
    {
        // provides a way to interact with users (particularly finding and updating in this case)
        private readonly UserManager<ApplicationUser> _userManager;

        public EditModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        // inputs from forms are automatically bound to Input
        // because of the bind property decorator
        [BindProperty]
        public InputModel Input { get; set; }

        // runs when page is loaded (GET request)
        public async Task<IActionResult> OnGetAsync()
        {
            // grab the user that is logged in
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user");
            }

            // get the info from the user model and populate an input model with it
            Input = new InputModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Degree = user.Degree,
                Introduction = user.Introduction,
                Specialisation = user.Specialisation,
                LinkedIn = user.LinkedIn,
                Resume = user.Resume,
                CoverLetter = user.CoverLetter,
                PreferredName = user.PreferedFirstName
            };

            // display page with user's existing data
            return Page();
        }

        // method runs on a POST request
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user");
            }

            // update the user from the input model
            user.FirstName = Input.FirstName;
            user.LastName = Input.LastName;
            user.Degree = Input.Degree;
            user.Introduction = Input.Introduction;
            user.Specialisation = Input.Specialisation;
            user.LinkedIn = Input.LinkedIn;
            user.Resume = Input.Resume;
            user.CoverLetter = Input.CoverLetter;
            user.PreferedFirstName = Input.PreferredName;

            // check if profile image has been uploaded
            if (Input.ProfileImage != null && Input.ProfileImage.Length > 0)
            {
                // using makes sure that after the byte array is stored in the user model
                // the memory allocated is freed up again
                using (var memoryStream = new MemoryStream())
                {
                    await Input.ProfileImage.CopyToAsync(memoryStream);

                    // convert memory stream to a byte array
                    user.ProfileImage = memoryStream.ToArray();
                }
            }

            // save the updated user to the database
            var result = await _userManager.UpdateAsync(user);

            // if update fails...
            if (!result.Succeeded)
            {
                // add errors to model state so they can be displayed on the page
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }

            return RedirectToPage("/Profile/Profile");
        }
    }

    // InputModel is only used on this page so imma keep it here
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
        public string? CoverLetter { get; set; }
        public string? PreferredName { get; set; }
    }
}