using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StudentPortfolio.Models;

public partial class Competency
{
    public long CompetencyId { get; set; }

    public String? CompetencyDisplayId { get; set; }

    public long? ParentCompetencyId { get; set; }

    public string Description { get; set; } = null!;

    public string? LinkToIndicators { get; set; }

    public DateTime LastUpdated { get; set; }

    public DateOnly? EndDate { get; set; }

    [ValidateNever]
    public virtual ICollection<CompetencyTracker> CompetencyTrackers { get; set; } = [];
}
