using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StudentPortfolio.Models;

public partial class Goal
{
    public long GoalId { get; set; }

    public string UserId { get; set; } = null!;

    public DateOnly DateSet { get; set; }

    public string Description { get; set; } = null!;

    public string? Timeline { get; set; }

    public string? Progress { get; set; }

    public string? Learnings { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public DateOnly? CompleteDate { get; set; }

    public string? CompletionNotes { get; set; }

    public virtual ICollection<GoalStep> GoalSteps { get; set; } = [];

    public virtual ApplicationUser User { get; set; } = null!;
}
