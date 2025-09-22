using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using StudentPortfolio.Data;
using StudentPortfolio.Models;

namespace StudentPortfolio.Pages.Networking
{
    public class CreateQuestionsModel : PageModel
    {
        private readonly StudentPortfolio.Data.ApplicationDbContext _context;

        public CreateQuestionsModel(StudentPortfolio.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["EventId"] = new SelectList(_context.NetworkingEvents, "EventId", "EventId");
            return Page();
        }

        [BindProperty]
        public NetworkingQuestion NetworkingQuestion { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.NetworkingQuestions.Add(NetworkingQuestion);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
