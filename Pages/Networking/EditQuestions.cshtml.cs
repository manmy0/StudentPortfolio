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
        public NetworkingQuestion NetworkingQuestion { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var networkingquestion =  await _context.NetworkingQuestions.FirstOrDefaultAsync(m => m.NetworkingQuestionsId == id);
            if (networkingquestion == null)
            {
                return NotFound();
            }
            NetworkingQuestion = networkingquestion;
           ViewData["EventId"] = new SelectList(_context.NetworkingEvents, "EventId", "EventId");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(NetworkingQuestion).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NetworkingQuestionExists(NetworkingQuestion.NetworkingQuestionsId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool NetworkingQuestionExists(long id)
        {
            return _context.NetworkingQuestions.Any(e => e.NetworkingQuestionsId == id);
        }
    }
}
