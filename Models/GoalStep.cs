using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentPortfolio.Models;

public partial class GoalStep
{
    public long StepId { get; set; }

    [Required]
    public long GoalId { get; set; }

    public string Step { get; set; } = null!;

    [ForeignKey("GoalId")]
    public virtual Goal Goal { get; set; } = null!;
}
