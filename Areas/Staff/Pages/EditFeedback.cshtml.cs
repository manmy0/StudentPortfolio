using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentPortfolio.Models;
using System.Linq;

namespace StudentPortfolio.Areas.Staff.Pages
{
    public class EditFeedbackModel : PageModel
    {
        private readonly StudentPortfolio.Data.ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public EditFeedbackModel(StudentPortfolio.Data.ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;

        }

        [BindProperty(Name = "feedbackId", SupportsGet = true)]
        public int? FeedbackId { get; set; }

        [BindProperty]
        public Feedback Feedback { get; set; } = new Feedback();

        public long? GoalId => Feedback?.GoalId;
        public long? CompetencyTrackerId => Feedback?.CompetencyTrackerId;

        public Goal SelectedGoal { get; set; } = default!;

        public IList<GoalStep> GoalSteps { get; set; } = default!;

        public CompetencyTracker SelectedCompetencyRecord { get; set; } = default!;

        public Competency SelectedCompetency { get; set; } = default!;

        private async Task LoadGoalDataAsync()
        {
            // Check if GoalId has a value
            if (GoalId.HasValue)
            {
                SelectedGoal = await _context.Goals.SingleOrDefaultAsync(i => i.GoalId == GoalId);

                GoalSteps = await _context.GoalSteps
                                        .Where(i => i.GoalId == GoalId)
                                        .ToListAsync();
            }
        }

        private async Task LoadCompetencyDataAsync()
        {
            // Check if CompetencyTrackerId has a value
            if (CompetencyTrackerId.HasValue)
            {
                SelectedCompetencyRecord = await _context.CompetencyTrackers
                    .Include(t => t.Level)
                    .SingleOrDefaultAsync(i => i.CompetencyTrackerId == CompetencyTrackerId);

                if (SelectedCompetencyRecord != null)
                {
                    SelectedCompetency = await _context.Competencies.SingleOrDefaultAsync(i => i.CompetencyId == SelectedCompetencyRecord.CompetencyId);
                }
            }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            Feedback = await _context.Feedbacks.FirstOrDefaultAsync(f => f.FeedbackId == FeedbackId);

            if (Feedback == null)
            {
                return NotFound();
            }

            // Checks if the Goalid or CompetencyTrackerId is not null inside the feedback object
            // and loads the correct function
            if (Feedback.GoalId.HasValue)
            {
                await LoadGoalDataAsync();
            }
            else if (Feedback.CompetencyTrackerId.HasValue)
            {
                await LoadCompetencyDataAsync();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // If the model is not valid, it will reload the page with the selected goal or competency data
            if (!ModelState.IsValid)
            {
                if (GoalId.HasValue)
                {
                    await LoadGoalDataAsync();
                }
                else if (CompetencyTrackerId.HasValue)
                {
                    await LoadCompetencyDataAsync();
                }

                return Page();
            }

            var existingFeedback = await _context.Feedbacks.FirstOrDefaultAsync(f => f.FeedbackId == FeedbackId);

            existingFeedback.FeedbackText = Feedback.FeedbackText;

            _context.Feedbacks.Update(existingFeedback);
            await _context.SaveChangesAsync();

            return RedirectToPage("/FeedbackDashboard");
        }

    }
}
