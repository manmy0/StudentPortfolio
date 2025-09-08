using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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
    [Authorize]
    public class NetworkingModel : PageModel
    {
        private readonly StudentPortfolio.Data.ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public NetworkingModel(StudentPortfolio.Data.ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public ApplicationUser CurrentUser { get; set; }
        public IList<IndustryContactLog> IndustryContactLog { get;set; } = default!;
        public IList<IndustryContactInfo> IndustryContactInfo { get; set; } = default!;
        public IList<NetworkingEvent> NetworkingEvent { get; set; } = default!;
        public IList<NetworkingQuestion> NetworkingQuestion { get; set; } = default!;

        public async Task OnGetAsync()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId != null)
            {
                CurrentUser = await _userManager.FindByIdAsync(userId);

                IndustryContactLog = await _context.IndustryContactLogs
                   .Where(i => i.UserId == userId)
                   .Include(i => i.User)
                   .ToListAsync();

                var contactIds = await _context.IndustryContactLogs
                                       .Where(i => i.UserId == userId)
                                       .Select(i => i.ContactId)
                                       .ToListAsync();

                IndustryContactInfo = await _context.IndustryContactInfos
                    .Where(i => contactIds.Contains(i.ContactId))
                    .ToListAsync();


                NetworkingEvent = await _context.NetworkingEvents
                    .Where(i => i.UserId == userId)
                    .Include(i => i.User)
                    .ToListAsync();


                var eventIds = await _context.NetworkingEvents
                    .Where(i => i.UserId == userId)
                    .Select(i => i.EventId)
                    .ToListAsync();


                NetworkingQuestion = await _context.NetworkingQuestions
                    .Where(i => eventIds.Contains(i.EventId))
                    .ToListAsync();
            }
        }
    }
}
