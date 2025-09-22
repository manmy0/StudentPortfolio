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
    public class EditEventModel : PageModel
    {
        private readonly StudentPortfolio.Data.ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public EditEventModel(StudentPortfolio.Data.ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public NetworkingEvent NetworkingEvent { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var networkingevent =  await _context.NetworkingEvents.FirstOrDefaultAsync(m => m.EventId == id);
           
            if (networkingevent == null)
            {
                return NotFound();
            }

            NetworkingEvent = networkingevent;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            NetworkingEvent.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(NetworkingEvent).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NetworkingEventExists(NetworkingEvent.EventId))
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

        private bool NetworkingEventExists(long id)
        {
            return _context.NetworkingEvents.Any(e => e.EventId == id);
        }
    }
}
