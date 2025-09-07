using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StudentPortfolio.Models;

public partial class GoalStep
{
    public long StepId { get; set; }

    public long GoalId { get; set; }

    public string Step { get; set; } = null!;

    public virtual Goal Goal { get; set; } = null!;
}
