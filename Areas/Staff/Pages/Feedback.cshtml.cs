using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentPortfolio.Models;
using StudentPortfolio.Pages.Goals;

namespace StudentPortfolio.Areas.Staff.Pages
{
    public class FeedbackModel : PageModel
    {
        private readonly StudentPortfolio.Data.ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public FeedbackModel(StudentPortfolio.Data.ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;

        }

        [BindProperty(SupportsGet = true)]
        public int? goalId { get; set; }

        public string Feedback {  get; set; }

        public Goal SelectedGoal { get; set; }

        public IList<GoalStep> GoalSteps { get; set; }

        public async Task OnGetAsync()
        {
            SelectedGoal = await _context.Goals.SingleOrDefaultAsync(i => i.GoalId == goalId);

            GoalSteps = await _context.GoalSteps
                        .Where(i => i.GoalId == goalId)
                        .ToListAsync();

        }

        public async Task<IActionResult> OnPostAsync()
        {
            return Page();



        }
    }
}
