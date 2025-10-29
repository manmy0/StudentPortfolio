namespace StudentPortfolio.Models.Staff_Models
{
    public class CompetencyData
    {
        public IList<Competency> Competencies { get; set; }
        public IList<Competency> ParentCompetencies { get; set; }
        public IList<CompetencyTracker> CompetencyTrackers { get; set; }

    }
}
