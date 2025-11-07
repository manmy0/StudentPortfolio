using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using StudentPortfolio.Models;
using StudentPortfolio.Models.Staff_Models;

namespace StudentPortfolio.Areas.Staff.Pages.Students
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

        public ProfileData ProfileData { get; set; }

        public GoalsData GoalsData { get; set; }

        public NetworkingData NetworkingData { get; set; }

        public CompetencyData CompetencyData { get; set; }



        public async Task OnGetAsync()
        {
            var user = await _userManager.FindByNameAsync(UserName);

            if (User == null)
            {
                return;
            }

            
            // Get all the years from each area
            var cdpYears = await _context.CareerDevelopmentPlans
                   .Where(i => i.UserId == user.Id)
                   .Select(c => c.Year)
                   .ToListAsync();

            var goalYears = await _context.Goals
                    .Where(i => i.UserId == user.Id && i.StartDate.HasValue)
                    .Select(i => (short)i.StartDate.Value.Year)
                    .ToListAsync();

            var networkingYears = await _context.NetworkingEvents
                    .Where(i => i.UserId == user.Id)
                    .Select(i => (short)i.Date.Value.Year)
                    .ToListAsync();

            var competencyYears = await _context.CompetencyTrackers
                    .Where(i => i.UserId == user.Id && i.StartDate.Year != null)
                    .Select(i => (short)i.StartDate.Year)
                    .ToListAsync();

            // Combine all years
            var allYears = cdpYears
                .Union(goalYears)
                .Union(networkingYears)
                .Union(competencyYears);

            // Get distinct years
            AvailableYears = allYears
                 .Distinct()
                 .OrderByDescending(y => y)
                 .ToList();


            // Determine which year to display
            if (selectedYear == null && AvailableYears.Any())
            {
                // If no year is selected, default to the most recent year
                selectedYear = AvailableYears.First();
            }   
            
 
            var allCDPs = await _context.CareerDevelopmentPlans
                        .Where(i => i.UserId == user.Id)
                        .OrderByDescending(c => c.Year) 
                        .ToListAsync();

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

            var links = await _context.UserLinks
                .Where(i => i.UserId == user.Id)
                .ToListAsync();

            ProfileData = new ProfileData
            {
                User = user,
                ProfileImageBase64 = user?.ProfileImage != null
                ? $"data:image/jpeg;base64,{Convert.ToBase64String(user.ProfileImage)}"
                : null,
                CDP = CDP,
                UserLinks = links
            };

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
            GoalsData = new GoalsData
            {
                GoalData = filteredGoals,
                GoalStepData = filteredGoalSteps
            };

            List<NetworkingEvent> filteredEvents = new List<NetworkingEvent>();
            List<NetworkingQuestion> filteredQuestions = new List<NetworkingQuestion>();
            List<IndustryContactLog> filteredContacts = new List<IndustryContactLog>();
            List<IndustryContactInfo> filteredInfo = new List<IndustryContactInfo>();


            filteredEvents = await _context.NetworkingEvents
                .Where(i => i.UserId == user.Id &&
                    i.Date.Value.Year == selectedYear.Value)
                .ToListAsync();

            var eventIds = filteredEvents.Select(i => i.EventId).ToList();

            if (eventIds.Any())
            {
                filteredQuestions = await _context.NetworkingQuestions
                    .Where(i => eventIds.Contains(i.EventId))
                    .ToListAsync();
            }

            filteredContacts = await _context.IndustryContactLogs
                .Where(i => i.UserId == user.Id 
                        && i.DateMet.Value.Year == selectedYear.Value)
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
                User = user,
                NetworkingContacts = filteredContacts,
                NetworkingContactInfo = filteredInfo,
                NetworkingQuestions = filteredQuestions,
                NetworkingEvents = filteredEvents

            };

            List<Competency> Competencies = new List<Competency>();
            List<Competency> ParentCompetencies = new List<Competency>();
            List<CompetencyTracker> CompetencyTrackers = new List<CompetencyTracker>();
            DateOnly currentDate = DateOnly.FromDateTime(DateTime.Now);

            if (selectedYear != null)
            {
                CompetencyTrackers = await _context.CompetencyTrackers
                               .Include(i => i.Level)
                               .Include(i => i.Competency)
                               .Where(i => i.UserId == user.Id &&
                                           i.StartDate.Year != null &&
                                           i.StartDate.Year == selectedYear.Value)
                               .Where(i => i.Competency.EndDate > currentDate || i.Competency.EndDate == null)
                               .OrderBy(i => i.CompetencyId)
                               .ThenByDescending(i => i.Level.Rank)
                               .ThenByDescending(i => i.StartDate)
                               .ThenByDescending(i => i.EndDate)
                               .ToListAsync();
            }

            var allCompetencies = await _context.Competencies
                .Where(c => c.EndDate > currentDate || c.EndDate == null)
                .ToListAsync();

            Competencies = allCompetencies;

            ParentCompetencies = allCompetencies.Where(c => c.ParentCompetencyId == null).ToList();

            CompetencyData = new CompetencyData
            {
                Competencies = Competencies,
                ParentCompetencies = ParentCompetencies,
                CompetencyTrackers = CompetencyTrackers

            };

        }



    }
}


