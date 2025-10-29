using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentPortfolio.Models;
using StudentPortfolio.Pages.Goals;
using System.Security.Claims;

namespace StudentPortfolio.Areas.Staff.Pages
{
    [Authorize(Roles="Staff")]
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

        public ApplicationUser CurrentUser { get; set; }

        public Goal SelectedGoal { get; set; }

        [BindProperty]
        public Feedback Feedback { get; set; } = new Feedback();

        public IList<GoalStep> GoalSteps { get; set; }

        private async Task LoadGoalDataAsync()
        {

            SelectedGoal = await _context.Goals.SingleOrDefaultAsync(i => i.GoalId == goalId);

            GoalSteps = await _context.GoalSteps
                                .Where(i => i.GoalId == goalId)
                                .ToListAsync();
        }

        public async Task OnGetAsync()
        {
            await LoadGoalDataAsync();

        }

        public async Task<IActionResult> OnPostAsync()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return NotFound();
            }

            CurrentUser = await _userManager.FindByIdAsync(userId);

            Feedback.UserId = CurrentUser.Id;
            Feedback.GoalId = goalId;

            if (!ModelState.IsValid)
            {
                await LoadGoalDataAsync();
                return Page();

            }

            _context.Feedbacks.Add(Feedback);
            await _context.SaveChangesAsync();
            return RedirectToPage("/Students");

        }
    }
}
