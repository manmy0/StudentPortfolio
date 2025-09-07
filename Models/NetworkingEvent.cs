using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StudentPortfolio.Models;

public partial class NetworkingEvent
{
    public long EventId { get; set; }

    public string UserId { get; set; } = null!;

    public string Name { get; set; } = null!;

    public DateTime? Date { get; set; }

    public string? Location { get; set; }

    public string? Details { get; set; }

    public virtual ICollection<NetworkingQuestion> NetworkingQuestions { get; set; } = [];

    public virtual ApplicationUser User { get; set; } = null!;

}