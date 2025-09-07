using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StudentPortfolio.Models;

public partial class IndustryContactInfo
{
    public long ContactInfoId { get; set; }

    public long ContactId { get; set; }

    public string? ContactType { get; set; }

    public string ContactDetails { get; set; } = null!;

    public virtual IndustryContactLog Contact { get; set; } = null!;
}
