using Microsoft.AspNetCore.Identity;

namespace StudentPortfolio.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? PreferedFirstName { get; set; }

        public string? Degree { get; set; }

        public string? Introduction { get; set; }

        public string? Pitch { get; set; }

        public string? Resume { get; set; }

        public string? CoverLetter { get; set; }

        public string? LinkedIn { get; set; }

        public string? Certificates { get; set; }

        public string? Specialisation { get; set; }

        public byte[]? ProfileImage { get; set; }

        public virtual required ICollection<CareerDevelopmentPlan> CareerDevelopmentPlans { get; set; }

        public virtual required ICollection<CompetencyTracker> CompetencyTrackers{ get; set; }

        public virtual required ICollection<Goal> Goals { get; set; }

        public virtual required ICollection<IndustryContactLog> IndustryContactLogs { get; set; }

        public virtual required ICollection<NetworkingEvent> NetworkingEvents { get; set; }

        public virtual required ICollection<UserLink> UserLinks { get; set; }

        public virtual required ICollection<Feedback> Feedbacks { get; set; }

        public String GetFirstName()
        {
            if (!string.IsNullOrWhiteSpace(this.PreferedFirstName))
            {
                return this.PreferedFirstName;
            }
            else if (!string.IsNullOrWhiteSpace(this.FirstName))
            {
                return this.FirstName;
            }
            return null;
        }
    }


}