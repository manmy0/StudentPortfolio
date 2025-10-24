using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StudentPortfolio.Models;

public partial class CareerDevelopmentPlan
{
    [ValidateNever]
    public string UserId { get; set; } = null!;

    public short Year { get; set; }

    public string? ProfessionalInterests { get; set; }

    public string? EmployersOfInterest { get; set; }

    public string? PersonalValues { get; set; }

    public string? DevelopmentFocus { get; set; }

    public string? Extracurricular { get; set; }

    public string? NetworkingPlan { get; set; }

    [ValidateNever]
    public virtual ApplicationUser User { get; set; } = null!;

}
