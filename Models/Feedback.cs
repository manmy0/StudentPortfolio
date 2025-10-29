using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace StudentPortfolio.Models
{
    public class Feedback
    {
        public int FeedbackId { get; set; }

        [ValidateNever]
        public string UserId { get; set; }

        [ValidateNever]
        public long? GoalId { get; set; }

        [ValidateNever]
        public long? CompetencyTrackerId { get; set; }

        public string FeedbackText { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateUpdated { get; set; }

        [ValidateNever]
        public virtual ApplicationUser User { get; set; } = null!;

        [ValidateNever]
        public virtual Goal Goal { get; set; } = null!;

        [ValidateNever]
        public virtual CompetencyTracker CompetencyTracker { get; set; } = null!;


    }
}
