using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using StudentPortfolio.Data;
using StudentPortfolio.Models;

namespace StudentPortfolio.Pages.Goals
{
    public class CreateStepsModel : PageModel
    {
        private readonly StudentPortfolio.Data.ApplicationDbContext _context;

        public CreateStepsModel(StudentPortfolio.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["GoalId"] = new SelectList(_context.Goals, "GoalId", "Description");
            return Page();
        }

        [BindProperty]
        public GoalStep GoalStep { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.GoalSteps.Add(GoalStep);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
