using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentPortfolio.Data;
using StudentPortfolio.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace StudentPortfolio.Areas.Staff.Pages
{
    [Authorize(Roles = "Staff")]
    public class FeedbackDashboardModel : PageModel
    {
        private readonly StudentPortfolio.Data.ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;


        public FeedbackDashboardModel(StudentPortfolio.Data.ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public List<StudentFeedbackViewModel> StudentFeedback { get; set; } = new List<StudentFeedbackViewModel>();

        public IList<Feedback> GoalFeedback { get;set; } = default!;

        public IList<Feedback> CompFeedback { get; set; } = default!;
        public ApplicationUser CurrentUser { get; set; }

        public async Task OnGetAsync()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            CurrentUser = await _userManager.FindByIdAsync(userId);

            // Gets all the goal feedback
            var allGoalFeedback = await _context.Feedbacks
                .Where(f => f.UserId == userId && f.Goal != null)
                .Include(f => f.Goal)
                    .ThenInclude(g => g.User) 
                .ToListAsync();

            // Gets all the competency feedback
            var allCompFeedback = await _context.Feedbacks
                .Where(f => f.UserId == userId && f.CompetencyTracker != null)
                .Include(f => f.CompetencyTracker)
                    .ThenInclude(ct => ct.User) 
                .Include(f => f.CompetencyTracker)
                    .ThenInclude(ct => ct.Competency) 
                .ToListAsync();

            // Checks nulls in case
            var goalStudents = allGoalFeedback
                .Where(f => f.Goal?.User != null)
                .Select(f => f.Goal.User);

            var compStudents = allCompFeedback
                .Where(f => f.CompetencyTracker?.User != null) 
                .Select(f => f.CompetencyTracker.User);

            
            var allStudents = goalStudents.Union(compStudents).Distinct(); // Gets unique list of students

            StudentFeedback = new List<StudentFeedbackViewModel>();

            foreach (var student in allStudents)
            {
                var studentModel = new StudentFeedbackViewModel
                {
                    Student = student,

                    // Find all feedback for the specific student
                    GoalFeedback = allGoalFeedback
                                     .Where(f => f.Goal?.UserId == student.Id)
                                     .ToList(),
                    CompFeedback = allCompFeedback
                                     .Where(f => f.CompetencyTracker?.UserId == student.Id)
                                     .ToList()
                };
                StudentFeedback.Add(studentModel);
            }

        }
    }
}

public class StudentFeedbackViewModel
{
    public ApplicationUser Student { get; set; }
    public List<Feedback> GoalFeedback { get; set; } = new List<Feedback>();
    public List<Feedback> CompFeedback { get; set; } = new List<Feedback>();
}