using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentPortfolio.Data;
using StudentPortfolio.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace StudentPortfolio.Pages.Networking
{
    public class EditContactModel : PageModel
    {
        private readonly StudentPortfolio.Data.ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        private const string PhoneType = "Phone";
        private const string EmailType = "Email";
        private const string LinkedInType = "LinkedIn";

        public EditContactModel(StudentPortfolio.Data.ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public IndustryContactLog IndustryContactLog { get; set; } = default!;

        public List<IndustryContactInfo> IndustryContactInfo { get; set; }  = new List<IndustryContactInfo>();

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

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

           var industrycontactlog = await _context.IndustryContactLogs
                .Include(log => log.IndustryContactInfos)
                .FirstOrDefaultAsync(m => m.ContactId == id);

            if (industrycontactlog == null)
            {
                return NotFound();
            }

            IndustryContactLog = industrycontactlog;

            PhoneNumber = industrycontactlog.IndustryContactInfos
                .FirstOrDefault(info => info.ContactType == PhoneType)?.ContactDetails;

            EmailAddress = industrycontactlog.IndustryContactInfos
                .FirstOrDefault(info => info.ContactType == EmailType)?.ContactDetails;

            LinkedInUrl = industrycontactlog.IndustryContactInfos
                .FirstOrDefault(info => info.ContactType == LinkedInType)?.ContactDetails;

            var otherInfo = industrycontactlog.IndustryContactInfos
                .FirstOrDefault(info => info.ContactType != PhoneType && info.ContactType != EmailType && info.ContactType != LinkedInType);

            if (otherInfo != null)
            {
                OtherContactType = otherInfo.ContactType;
                OtherContactDetails = otherInfo.ContactDetails;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            IndustryContactLog.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(IndustryContactLog).State = EntityState.Modified;

            await UpdateContactInfoAsync(IndustryContactLog.ContactId, PhoneType, PhoneNumber);
            await UpdateContactInfoAsync(IndustryContactLog.ContactId, EmailType, EmailAddress);
            await UpdateContactInfoAsync(IndustryContactLog.ContactId, LinkedInType, LinkedInUrl);

            var existingOther = await _context.IndustryContactInfos
                .FirstOrDefaultAsync(i => i.ContactId == IndustryContactLog.ContactId &&
                                           i.ContactType != PhoneType &&
                                           i.ContactType != EmailType &&
                                           i.ContactType != LinkedInType);
            if (existingOther != null)
            {
                _context.IndustryContactInfos.Remove(existingOther);
            }
          
            if (!string.IsNullOrWhiteSpace(OtherContactType) && !string.IsNullOrWhiteSpace(OtherContactDetails))
            {
                _context.IndustryContactInfos.Add(new IndustryContactInfo
                {
                    ContactId = IndustryContactLog.ContactId,
                    ContactType = OtherContactType,
                    ContactDetails = OtherContactDetails
                });
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!IndustryContactLogExists(IndustryContactLog.ContactId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("Networking");
        }

        private async Task UpdateContactInfoAsync(long contactId, string contactType, string? newDetails)
        {
            var existingInfo = await _context.IndustryContactInfos
                .FirstOrDefaultAsync(i => i.ContactId == contactId && i.ContactType == contactType);

            if (!string.IsNullOrWhiteSpace(newDetails))
            {
                if (existingInfo != null)
                {
                    existingInfo.ContactDetails = newDetails;
                }
                else
                {
                    _context.IndustryContactInfos.Add(new IndustryContactInfo
                    {
                        ContactId = contactId,
                        ContactType = contactType,
                        ContactDetails = newDetails
                    });
                }
            }
            else if (existingInfo != null)
            {
                _context.IndustryContactInfos.Remove(existingInfo);
            }
        }

        private bool IndustryContactLogExists(long id)
        {
            return _context.IndustryContactLogs.Any(e => e.ContactId == id);
        }
    }

}
