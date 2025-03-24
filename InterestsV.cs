using System;
using System.Collections.Generic;

namespace MyAPI;

public partial class InterestsV
{
    public int? Id { get; set; }

    public int? ProductId { get; set; }

    public DateTime? OrderDate { get; set; }

    public string? Description { get; set; }

    public decimal? Amount { get; set; }

    public int? Quantity { get; set; }

    public decimal? Total { get; set; }

    public decimal? Interest { get; set; }
}
