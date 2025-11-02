using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentPortfolio.Models;

namespace StudentPortfolio.Areas.Admin.Pages.CDL
{
    public class CreateCDLModel : PageModel
    {
        private readonly StudentPortfolio.Data.ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CreateCDLModel(StudentPortfolio.Data.ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public Cdl Cdl { get; set; } = new Cdl();

        public IFormFile? IconImage { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            Cdl.DateCreated = DateTime.Now;
            Cdl.LastUpdated = DateTime.Now;

            // check if profile image has been uploaded
            if (IconImage != null && IconImage.Length > 0)
            {
                // using makes sure that after the byte array is stored in the user model
                // the memory allocated is freed up again
                using (var memoryStream = new MemoryStream())
                {
                    await IconImage.CopyToAsync(memoryStream);

                    // convert memory stream to a byte array
                    Cdl.IconImage = memoryStream.ToArray();
                }
            }

            _context.Cdls.Add(Cdl);
            await _context.SaveChangesAsync();
            return RedirectToPage("./CDLs");
        }
    }
}
