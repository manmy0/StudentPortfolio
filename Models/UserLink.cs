using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StudentPortfolio.Models;

public partial class UserLink
{
    public long LinkId { get; set; }

    public string UserId { get; set; } = null!;

    public string Link { get; set; } = null!;

    public virtual ApplicationUser User { get; set; } = null!;
}
