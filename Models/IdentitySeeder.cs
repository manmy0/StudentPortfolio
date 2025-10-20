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

            // Check if the admin user already exists
            var userExist = await userManager.FindByEmailAsync(adminEmail);
            if (userExist == null)
            {
                var adminUser = new ApplicationUser
                {
                    UserName = "admin@gmail.com",
                    Email = adminEmail,
                    FirstName = "Admin",
                    EmailConfirmed = true,
                    CareerDevelopmentPlans = new List<CareerDevelopmentPlan>(),
                    CompetencyTrackers = new List<CompetencyTracker>(),
                    ContactsOfInterests = new List<ContactsOfInterest>(),
                    Goals = new List<Goal>(),
                    IndustryContactLogs = new List<IndustryContactLog>(),
                    NetworkingEvents = new List<NetworkingEvent>(),
                    UserLinks = new List<UserLink>()

                };

                // Create the admin user
                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    // Assign the Admin role to the user
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
                else
                {
                    throw new Exception("Failed to create the admin user: " + string.Join(", ", result.Errors));
                }
            }
        }
    }
}
