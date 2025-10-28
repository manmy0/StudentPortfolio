using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentPortfolio.Models;

namespace StudentPortfolio.Areas.Staff.Pages
{
    public class ViewStudentDataModel : PageModel
    {
        private readonly StudentPortfolio.Data.ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ViewStudentDataModel(StudentPortfolio.Data.ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;

        }

        [BindProperty(Name = "userName", SupportsGet = true)]
        public string UserName { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? selectedYear { get; set; }

        public CareerDevelopmentPlan? CDP { get; set; } = default!;

        public IList<short> AvailableYears { get; set; } = default!;

        public SmartGoals ViewModel { get; set; }

        public NetworkingData NetworkingData { get; set; }

        public async Task OnGetAsync()
        {
            var user = await _userManager.FindByNameAsync(UserName);

            if (user == null)
            {
                return;
            }

            var allCDPs = await _context.CareerDevelopmentPlans
                        .Where(i => i.UserId == user.Id)
                        .OrderByDescending(c => c.Year) 
                        .ToListAsync();

            AvailableYears = allCDPs.Select(c => c.Year).Distinct().ToList();

            // Determine which year to display
            if (selectedYear == null && AvailableYears.Any())
            {
                // If no year is selected, default to the most recent year
                selectedYear = AvailableYears.First();
            }

            // Find the specific CDP for the selected year
            if (selectedYear != null)
            {
                CDP = allCDPs.FirstOrDefault(c => c.Year == selectedYear);
            }
            else
            {
                // No CDPs exist for this user
                CDP = null;
            }

            List<Goal> filteredGoals = new List<Goal>();
            List<GoalStep> filteredGoalSteps = new List<GoalStep>();

            if (selectedYear != null)
            {
                // Fetch Goals filtered by the selectedYear.
                filteredGoals = await _context.Goals
                    .Where(i => i.UserId == user.Id && i.StartDate.Value.Year == selectedYear.Value)
                    .Include(i => i.User)
                    .ToListAsync();

                // Get the IDs from the filtered goals
                var goalIds = filteredGoals.Select(i => i.GoalId).ToList();

                // Fetch the steps for those filtered goals
                if (goalIds.Any())
                {
                    filteredGoalSteps = await _context.GoalSteps
                        .Where(i => goalIds.Contains(i.GoalId))
                        .ToListAsync();
                }
            }

            // 5. Assign the filtered lists to the ViewModel
            ViewModel = new SmartGoals
            {
                GoalData = filteredGoals,
                GoalStepData = filteredGoalSteps
            };

            List<NetworkingEvent> filteredEvents = new List<NetworkingEvent>();
            List<NetworkingQuestion> filteredQuestions = new List<NetworkingQuestion>();
            List<IndustryContactLog> filteredContacts = new List<IndustryContactLog>();
            List<IndustryContactInfo> filteredInfo = new List<IndustryContactInfo>();


            filteredEvents = await _context.NetworkingEvents
                .Where(i => i.UserId == user.Id)
                .ToListAsync();

            var eventIds = filteredEvents.Select(i => i.EventId).ToList();

            if (eventIds.Any())
            {
                filteredQuestions = await _context.NetworkingQuestions
                    .Where(i => eventIds.Contains(i.EventId))
                    .ToListAsync();
            }

            filteredContacts = await _context.IndustryContactLogs
                .Where(i => i.UserId == user.Id)
                .ToListAsync();

            var contactIds = filteredQuestions.Select(i => i.EventId).ToList();

            if (contactIds.Any())
            {
                filteredInfo = await _context.IndustryContactInfos
                    .Where(i => contactIds.Contains(i.ContactId))
                    .ToListAsync();
            }

            NetworkingData = new NetworkingData
            {
                NetworkingContacts = filteredContacts,
                NetworkingContactInfo = filteredInfo,
                NetworkingQuestions = filteredQuestions,
                NetworkingEvents = filteredEvents

            };

        }



    }
}


public class SmartGoals
{
    public IList<Goal> GoalData { get; set; }
    public IList<GoalStep> GoalStepData { get; set; }

}

public class NetworkingData
{
    public IList<IndustryContactLog> NetworkingContacts { get; set; }
    public IList<IndustryContactInfo> NetworkingContactInfo {  get; set; }
    public IList<NetworkingEvent> NetworkingEvents { get; set; }
    public IList<NetworkingQuestion> NetworkingQuestions { get; set; }
}
