using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StudentPortfolio.Models;

public partial class IndustryContactLog
{
    public long ContactId { get; set; }

    [ValidateNever]
    public string UserId { get; set; } = null!;

    public string? Name { get; set; }

    public string? Company { get; set; }

    public string? Notes { get; set; }

    public DateOnly? DateMet { get; set; }

    public virtual ICollection<IndustryContactInfo> IndustryContactInfos { get; set; } = [];

    [ValidateNever]
    public virtual ApplicationUser User { get; set; } = null!;
}
