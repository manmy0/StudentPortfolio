using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StudentPortfolio.Models;

public partial class NetworkingQuestion
{
    public long NetworkingQuestionsId { get; set; }

    public long EventId { get; set; }

    public string? Question { get; set; }

    [ValidateNever]
    public virtual NetworkingEvent Event { get; set; } = null!;
}
