namespace StudentPortfolio.Models
{
    public class Level
    {
        public long LevelId { get; set; }

        public string Name { get; set; } = null!;

        public short Rank { get; set; }

        public virtual ICollection<CompetencyTracker> CompetencyTrackers { get; set; } = [];
    }
}
