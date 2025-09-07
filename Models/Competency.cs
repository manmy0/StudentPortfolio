using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StudentPortfolio.Models;

public partial class Competency
{
    public long CompetencyId { get; set; }

    public long? ParentCompetencyId { get; set; }

    public string Description { get; set; } = null!;

    public string? LinkToIndicators { get; set; }

    public DateTime LastUpdated { get; set; }

    public virtual ICollection<CompetencyTracker> CompetencyTrackers { get; set; } = [];
}
