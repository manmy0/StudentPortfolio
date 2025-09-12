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
    public class EditStepModel : PageModel
    {
        private readonly StudentPortfolio.Data.ApplicationDbContext _context;

        public EditStepModel(StudentPortfolio.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty(Name = "goalId", SupportsGet = true)]
        public int Id { get; set; }

        [BindProperty]
        public List<GoalStep> GoalSteps { get; set; } = new List<GoalStep>();
        public Goal Goal { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Goal = await _context.Goals.FindAsync((long) Id);

            if (Goal == null)
            {
                return NotFound();
            }

            GoalSteps = await _context.GoalSteps
                .Where(s => s.GoalId == Id)
                .OrderBy(s => s.StepId)
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                Goal = await _context.Goals.FindAsync((long)Id);
                return Page();
            }

            var originalSteps = await _context.GoalSteps
               .Where(s => s.GoalId == Id)
               .AsNoTracking()
               .ToListAsync();

            var stepsToDelete = originalSteps
                .Where(dbStep => !GoalSteps.Any(formStep => formStep.StepId == dbStep.StepId))
                .ToList();

            _context.GoalSteps.RemoveRange(stepsToDelete);

            foreach (var submittedStep in GoalSteps)
            {
         
                if (string.IsNullOrWhiteSpace(submittedStep.Step))
                {
                    continue;
                }

                submittedStep.GoalId = Id; 

                if (submittedStep.StepId == 0)
                {
                    _context.GoalSteps.Add(submittedStep);
                }
              
                else
                {
                    _context.GoalSteps.Update(submittedStep);
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToPage("Goals");
        }

    }

}