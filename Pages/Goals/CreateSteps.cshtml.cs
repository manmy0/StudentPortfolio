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
    public class CreateStepsModel : PageModel
    {
        private readonly StudentPortfolio.Data.ApplicationDbContext _context;

        public CreateStepsModel(StudentPortfolio.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty(Name = "goalId", SupportsGet = true)]
        public int Id { get; set; }

        [BindProperty]
        public List<GoalStep> GoalSteps { get; set; } = new List<GoalStep>();

        public void OnGet()
        {
            GoalSteps.Add(new GoalStep());
        }
        public async Task<IActionResult> OnPostAsync()
        {
            var idExists = await _context.Goals.AnyAsync(g => g.GoalId == Id);

            if (!idExists)
            {
                ModelState.AddModelError("GoalSteps.GoalId", "Selected Category does not exist.");
                
                return Page(); 
            }

            if (GoalSteps.Count > 0)
            {
                foreach (var step in GoalSteps)
                {
                    step.GoalId = Id; 
                }
            }

            GoalSteps.RemoveAll(step => string.IsNullOrWhiteSpace(step.Step));

            _context.GoalSteps.AddRange(GoalSteps);
            await _context.SaveChangesAsync();

            return RedirectToPage("Goals");
        }
    }
}
