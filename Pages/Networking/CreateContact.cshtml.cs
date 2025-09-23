using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using StudentPortfolio.Data;
using StudentPortfolio.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace StudentPortfolio.Pages.Networking
{
    public class CreateContactModel : PageModel
    {
        private readonly StudentPortfolio.Data.ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;


        public CreateContactModel(StudentPortfolio.Data.ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public IndustryContactLog IndustryContactLog { get; set; } = default!;

        [BindProperty]
        public string? PhoneNumber { get; set; }

        [BindProperty]
        public string? EmailAddress { get; set; }

        [BindProperty]
        public string? LinkedInUrl { get; set; }

        [BindProperty]
        public string? OtherContactType { get; set; }

        [BindProperty]
        public string? OtherContactDetails { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            IndustryContactLog.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!ModelState.IsValid)
            {
                return Page();
            }

            IndustryContactLog.IndustryContactInfos = new List<IndustryContactInfo>();

            if (!string.IsNullOrWhiteSpace(PhoneNumber))
            {
                IndustryContactLog.IndustryContactInfos.Add(new IndustryContactInfo
                {
                    ContactType = "Phone",
                    ContactDetails = PhoneNumber
                });
            }

            if (!string.IsNullOrWhiteSpace(EmailAddress))
            {
                IndustryContactLog.IndustryContactInfos.Add(new IndustryContactInfo
                {
                    ContactType = "Email",
                    ContactDetails = EmailAddress
                });
            }

            if (!string.IsNullOrWhiteSpace(LinkedInUrl))
            {
                IndustryContactLog.IndustryContactInfos.Add(new IndustryContactInfo
                {
                    ContactType = "LinkedIn",
                    ContactDetails = LinkedInUrl
                });
            }

            if (!string.IsNullOrWhiteSpace(OtherContactDetails))
            {
                IndustryContactLog.IndustryContactInfos.Add(new IndustryContactInfo
                {
                    ContactType = string.IsNullOrWhiteSpace(OtherContactType) ? "Other" : OtherContactType,
                    ContactDetails = OtherContactDetails
                });
            }

            _context.IndustryContactLogs.Add(IndustryContactLog);
            await _context.SaveChangesAsync();

            return RedirectToPage("Networking");
        }
    }
}
