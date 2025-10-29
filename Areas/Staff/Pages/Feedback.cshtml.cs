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

        [BindProperty(Name="goalId", SupportsGet = true)]
        public int? GoalId { get; set; }

        [BindProperty(Name = "compId", SupportsGet = true)]
        public int? CompetencyTrackerId { get; set; }

        public ApplicationUser CurrentUser { get; set; }

        public Goal SelectedGoal { get; set; }

        public CompetencyTracker SelectedCompetencyRecord { get; set; }

        public Competency SelectedCompetency { get; set; }

        [BindProperty]
        public Feedback Feedback { get; set; } = new Feedback();

        public IList<GoalStep> GoalSteps { get; set; }

        private async Task LoadGoalDataAsync()
        {

            SelectedGoal = await _context.Goals.SingleOrDefaultAsync(i => i.GoalId == GoalId);

            GoalSteps = await _context.GoalSteps
                                .Where(i => i.GoalId == GoalId)
                                .ToListAsync();
        }

        private async Task LoadCompetencyDataAsync()
        {

            SelectedCompetencyRecord = await _context.CompetencyTrackers
                .Include(t => t.Level)
                .SingleOrDefaultAsync(i => i.CompetencyTrackerId == CompetencyTrackerId);

            if (SelectedCompetencyRecord != null)
            {
                SelectedCompetency = await _context.Competencies.SingleOrDefaultAsync(i => i.CompetencyId == SelectedCompetencyRecord.CompetencyId);

            }
        }

        public async Task OnGetAsync()
        {
            if (GoalId != null)
            {
                await LoadGoalDataAsync();
            }
            else if (CompetencyTrackerId != null)
            {
                await LoadCompetencyDataAsync();
            }
           

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

            if (GoalId != null)
            {
                Feedback.GoalId = GoalId;
            }
            else if (CompetencyTrackerId != null)
            {
                Feedback.CompetencyTrackerId = CompetencyTrackerId;
            }

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
