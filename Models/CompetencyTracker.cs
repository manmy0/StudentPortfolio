using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StudentPortfolio.Models;

public partial class CompetencyTracker
{
    public long CompetencyTrackerId { get; set; }

    public string UserId { get; set; } = null!;

    public long CompetencyId { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public long LevelId { get; set; }

    public string? SkillsReview { get; set; }

    public string? Evidence { get; set; }

    public DateTime Created { get; set; }

    public DateTime LastUpdated { get; set; }

    public virtual Competency Competency { get; set; } = null!;

    public virtual ApplicationUser User { get; set; } = null!;

    public virtual Level Level { get; set; } = null!;
}
