using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StudentPortfolio.Models;

public partial class Cdl
{
    public int CdlId { get; set; }

    public DateTime DateCreated { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string? Link { get; set; }

    [DataType(DataType.Date)]
    public DateTime LastUpdated { get; set; }

}
