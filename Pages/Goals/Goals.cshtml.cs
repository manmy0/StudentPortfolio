using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentPortfolio.Data;
using StudentPortfolio.Models;

namespace StudentPortfolio.Pages.Goals
{
    [Authorize]
    public class GoalsModel : PageModel
    {
        private readonly StudentPortfolio.Data.ApplicationDbContext _context;

        public GoalsModel(StudentPortfolio.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Goal> Goal { get;set; } = default!;
        public IList<CareerDevelopmentPlan> CDP { get; set; } = default!;
        public IList<GoalStep> GoalSteps { get; set; } = default!;

        public async Task OnGetAsync()
        {
            Goal = await _context.Goals
                .Include(g => g.User).ToListAsync();

            CDP = await _context.CareerDevelopmentPlans
                .Include(g => g.User).ToListAsync();

            GoalSteps = await _context.GoalSteps
                .Include(g => g.Goal).ToListAsync();
        }
    }
}
