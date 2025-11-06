using Microsoft.AspNetCore.Identity;
using System.Diagnostics.CodeAnalysis;

namespace StudentPortfolio.Models
{
    public static class IdentitySeeder
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // Define roles to seed
            var roles = new[] { "Admin", "Student", "Staff" };

            // Seed roles
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Define the admin user details
            var adminEmail = "admin@gmail.com";
            var adminPassword = "Admin@123";

            // Define student user details
            var studentEmail = "test@test.com";
            var studentPassword = "Test1@";

            // Define staff user details
            var staffEmail = "staff@staff.com";
            var staffPassword = "Staff1@";

            // Check if the users already exist
            var adminExist = await userManager.FindByEmailAsync(adminEmail);
            var studentExist = await userManager.FindByEmailAsync(studentEmail);
            var staffExist = await userManager.FindByEmailAsync(staffEmail);

            if (adminExist == null)
            {
                var adminUser = new ApplicationUser
                {
                    UserName = "admin@gmail.com",
                    Email = adminEmail,
                    FirstName = "Rose",
                    LastName = "Lalonde",
                    EmailConfirmed = true,
                    CareerDevelopmentPlans = new List<CareerDevelopmentPlan>(),
                    CompetencyTrackers = new List<CompetencyTracker>(),
                    ContactsOfInterests = new List<ContactsOfInterest>(),
                    Goals = new List<Goal>(),
                    IndustryContactLogs = new List<IndustryContactLog>(),
                    NetworkingEvents = new List<NetworkingEvent>(),
                    UserLinks = new List<UserLink>(),
                    Feedbacks = new List<Feedback>()

                };

                // Create the admin user
                var adminResult = await userManager.CreateAsync(adminUser, adminPassword);
                if (adminResult.Succeeded)
                {
                    // Assign the Admin role to the user
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
                else
                {
                    throw new Exception("Failed to create the admin user: " + string.Join(", ", adminResult.Errors));
                }
            }

            if (studentExist == null)
            {
                var studentUser = new ApplicationUser
                {
                    UserName = studentEmail,
                    Email = studentEmail,
                    FirstName = "John",
                    PreferedFirstName = "Johnny",
                    LastName = "Smith",
                    EmailConfirmed = true,
                    CareerDevelopmentPlans = new List<CareerDevelopmentPlan>(),
                    CompetencyTrackers = new List<CompetencyTracker>(),
                    ContactsOfInterests = new List<ContactsOfInterest>(),
                    Goals = new List<Goal>(),
                    IndustryContactLogs = new List<IndustryContactLog>(),
                    NetworkingEvents = new List<NetworkingEvent>(),
                    UserLinks = new List<UserLink>(),
                    Feedbacks = new List<Feedback>()

                };

                // Create the student user
                var studentResult = await userManager.CreateAsync(studentUser, studentPassword);
                if (studentResult.Succeeded)
                {
                    // Assign the student role to the user
                    await userManager.AddToRoleAsync(studentUser, "Student");
                }
                else
                {
                    throw new Exception("Failed to create the student user: " + string.Join(", ", studentResult.Errors));
                }

            }


            if (staffExist == null)
            {
                var staffUser = new ApplicationUser
                {
                    UserName = staffEmail,
                    Email = staffEmail,
                    FirstName = "Jane",
                    LastName = "Crocker",
                    EmailConfirmed = true,
                    CareerDevelopmentPlans = new List<CareerDevelopmentPlan>(),
                    CompetencyTrackers = new List<CompetencyTracker>(),
                    ContactsOfInterests = new List<ContactsOfInterest>(),
                    Goals = new List<Goal>(),
                    IndustryContactLogs = new List<IndustryContactLog>(),
                    NetworkingEvents = new List<NetworkingEvent>(),
                    UserLinks = new List<UserLink>(),
                    Feedbacks = new List<Feedback>()

                };

                // Create the staff user
                var staffResult = await userManager.CreateAsync(staffUser, staffPassword);
                if (staffResult.Succeeded)
                {
                    // Assign the staff role to the user
                    await userManager.AddToRoleAsync(staffUser, "Staff");
                }
                else
                {
                    throw new Exception("Failed to create the staff user: " + string.Join(", ", staffResult.Errors));
                }

            }
        }
    }
}
