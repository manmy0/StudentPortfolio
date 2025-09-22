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
using System.Threading.Tasks;

namespace StudentPortfolio.Pages.Networking
{
    public class CreateQuestionsModel : PageModel
    {
        private readonly StudentPortfolio.Data.ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;


        public CreateQuestionsModel(StudentPortfolio.Data.ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;

        }

        [BindProperty(Name = "eventId", SupportsGet = true)]
        public int Id { get; set; }


        [BindProperty]
        public List<NetworkingQuestion> NetworkingQuestions { get; set; } = new List<NetworkingQuestion>();

        public IList<NetworkingEvent> NetworkingEvent { get; set; } = default!;

        public async Task OnGet()
        {
            NetworkingQuestions.Add(new NetworkingQuestion());

            NetworkingEvent = await _context.NetworkingEvents
                .Where(g => g.EventId == Id)
                .ToListAsync();

        }

        public async Task<IActionResult> OnPostAsync()
        {
            var idExists = await _context.NetworkingEvents.AnyAsync(g => g.EventId == Id);
          
            if (!idExists)
            {
                ModelState.AddModelError("NetworkingEvents.EventId", "Selected Event does not exist.");

                return Page();
            }

            if (NetworkingQuestions.Count > 0)
            {
                foreach (var question in NetworkingQuestions)
                {
                    question.EventId = Id;
                }
            }


            _context.NetworkingQuestions.AddRange(NetworkingQuestions);
            await _context.SaveChangesAsync();

            return RedirectToPage("Networking");
        }
    }
}
