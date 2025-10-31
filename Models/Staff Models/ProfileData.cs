namespace StudentPortfolio.Models.Staff_Models
{
    public class ProfileData
    {
        public ApplicationUser User { get; set; }
        public string? ProfileImageBase64 { get; set; }

        public CareerDevelopmentPlan CDP { get; set; }

        public IList<UserLink> UserLinks { get; set; }
    }
}
