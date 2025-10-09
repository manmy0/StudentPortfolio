using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentPortfolio.Data;
using StudentPortfolio.Models;

namespace StudentPortfolio.Pages.Networking
{
    public class EditQuestionsModel : PageModel
    {
        private readonly StudentPortfolio.Data.ApplicationDbContext _context;

        public EditQuestionsModel(StudentPortfolio.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty(Name = "eventId", SupportsGet = true)]
        public int Id { get; set; }

        [BindProperty]
        public List<NetworkingQuestion> NetworkingQuestions { get; set; } = new List<NetworkingQuestion>();

        public NetworkingEvent NetworkingEvent { get; set; }

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            NetworkingEvent = await _context.NetworkingEvents.FindAsync((long)Id);

            if (NetworkingEvent == null)
            {
                return NotFound();
            }

            NetworkingQuestions = await _context.NetworkingQuestions
                .Where(s => s.EventId == Id)
                .OrderBy(s => s.NetworkingQuestionsId)
                .ToListAsync();

            return Page();

        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                NetworkingEvent = await _context.NetworkingEvents.FindAsync((long)Id);
                return Page();
            }

            var originalQuestions = await _context.NetworkingQuestions
               .Where(s => s.EventId == Id)
               .AsNoTracking()
               .ToListAsync();

            var NetworkingQuestionsToDelete = originalQuestions
                .Where(dbStep => !NetworkingQuestions.Any(formQuestion => formQuestion.NetworkingQuestionsId == dbStep.NetworkingQuestionsId))
                .ToList();

            _context.NetworkingQuestions.RemoveRange(NetworkingQuestionsToDelete);

            foreach (var submittedQuestion in NetworkingQuestions)
            {

                if (string.IsNullOrWhiteSpace(submittedQuestion.Question))
                {
                    continue;
                }

                submittedQuestion.EventId = Id;

                if (submittedQuestion.NetworkingQuestionsId == 0)
                {
                    _context.NetworkingQuestions.Add(submittedQuestion);
                }

                else
                {
                    _context.NetworkingQuestions.Update(submittedQuestion);
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToPage("Networking");
        }
    }
}
