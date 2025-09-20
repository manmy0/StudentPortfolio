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

        [BindProperty]
        public string? Pitch { get; set; } 
        public ApplicationUser CurrentUser { get; set; }
        public IList<IndustryContactLog> IndustryContactLog { get;set; } = default!;
        public IList<IndustryContactInfo> IndustryContactInfo { get; set; } = default!;
        public IList<NetworkingEvent> NetworkingEvent { get; set; } = default!;
        public IList<NetworkingQuestion> NetworkingQuestion { get; set; } = default!;

        private async Task LoadPageDataAsync(string userId)
        {
            IndustryContactLog = await _context.IndustryContactLogs
                .Where(i => i.UserId == userId)
                .Include(i => i.User)
                .ToListAsync();

            var contactIds = IndustryContactLog.Select(i => i.ContactId).ToList();

            IndustryContactInfo = await _context.IndustryContactInfos
                .Where(i => contactIds.Contains(i.ContactId))
                .ToListAsync();

            NetworkingEvent = await _context.NetworkingEvents 
                .Where(i => i.UserId == userId)
                .Include(i => i.User)
                .ToListAsync();

            var eventIds = NetworkingEvent.Select(i => i.EventId).ToList();

            NetworkingQuestion = await _context.NetworkingQuestions
                .Where(i => eventIds.Contains(i.EventId))
                .ToListAsync();
        }

        public async Task<IActionResult> OnGetAsync()
        {
            string userId = _userManager.GetUserId(User);
           
            if (userId == null)
            {
                return NotFound();
            }

            CurrentUser = await _userManager.FindByIdAsync(userId);
            Pitch = CurrentUser.Pitch;

            await LoadPageDataAsync(userId); 

            return Page();

        }

        public async Task<IActionResult> OnPostAsync()
        {
            string userId = _userManager.GetUserId(User);
           
            if (userId == null)
            {
                return NotFound();
            }

            var userToUpdate = await _userManager.FindByIdAsync(userId);

            if (userToUpdate == null)
            {
                return NotFound();

            }
            
            if (!ModelState.IsValid)
            {
                await LoadPageDataAsync(userId);
                return Page();
            }

            userToUpdate.Pitch = Pitch;

            var result = await _userManager.UpdateAsync(userToUpdate);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                await LoadPageDataAsync(userToUpdate.Id);
                
                return Page();
            }

            return RedirectToPage();

        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var eventToDelete = await _context.NetworkingEvents
                .FirstOrDefaultAsync(g => g.EventId == id && g.UserId == userId);

            if (eventToDelete == null)
            {
                return NotFound();
            }

            var questionsToDelete = await _context.NetworkingQuestions
                .Where(s => s.EventId == id)
                .ToListAsync();


            if (questionsToDelete.Any())
            {
                _context.NetworkingQuestions.RemoveRange(questionsToDelete);
            }

            _context.NetworkingEvents.Remove(eventToDelete);
            await _context.SaveChangesAsync();

            return RedirectToPage("Networking");
        }
    }
}

