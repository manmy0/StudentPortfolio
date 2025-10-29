namespace StudentPortfolio.Models
{
    public class Feedback
    {
        public int FeedbackId { get; set; }
        
        public string UserId { get; set; }

        public long? GoalId { get; set; }

        public long? CompetencyTrackerId { get; set; }

        public string FeedbackText { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateUpdated { get; set; }

        public virtual ApplicationUser User { get; set; } = null!;

        public virtual Goal Goal { get; set; } = null!;

        public virtual CompetencyTracker CompetencyTracker { get; set; } = null!;


    }
}
