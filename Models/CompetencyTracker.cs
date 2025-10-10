using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StudentPortfolio.Models;

public partial class CompetencyTracker
{
    public long CompetencyTrackerId { get; set; }

    [ValidateNever]
    public string UserId { get; set; } = null!;

    [ValidateNever]
    public long CompetencyId { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    [ValidateNever]
    public long LevelId { get; set; }

    public string? SkillsReview { get; set; }

    public string? Evidence { get; set; }

    public DateTime Created { get; set; }

    public DateTime LastUpdated { get; set; }

    [ValidateNever]
    public virtual Competency Competency { get; set; } = null!;

    [ValidateNever]
    public virtual ApplicationUser User { get; set; } = null!;

    [ValidateNever]
    public virtual Level Level { get; set; } = null!;
}
