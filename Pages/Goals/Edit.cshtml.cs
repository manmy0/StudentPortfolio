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

namespace StudentPortfolio.Pages.Goals
{
    public class EditModel : PageModel
    {
        private readonly StudentPortfolio.Data.ApplicationDbContext _context;

        public EditModel(StudentPortfolio.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Goal Goal { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var goal =  await _context.Goals.FirstOrDefaultAsync(m => m.GoalId == id);
            if (goal == null)
            {
                return NotFound();
            }

            Goal = goal;
            return Page();
        }

    
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Goal).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GoalExists(Goal.GoalId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("Goals");
        }

        private bool GoalExists(long id)
        {
            return _context.Goals.Any(e => e.GoalId == id);
        }
    }
}
