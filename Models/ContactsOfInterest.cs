using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StudentPortfolio.Models;

public partial class ContactsOfInterest
{
    public long ContactOfInterestId { get; set; }

    public string UserId { get; set; } = null!;

    public string ContactDetails { get; set; } = null!;

    public virtual ApplicationUser User { get; set; } = null!;
}
