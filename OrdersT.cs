using System;
using System.Collections.Generic;

namespace MyAPI;

public partial class OrdersT
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public int? Quantity { get; set; }

    public decimal? Amount { get; set; }

    public decimal? Total { get; set; }

    public DateTime OrderDate { get; set; }

    public DateTime CreateDate { get; set; }

    public DateTime ModifyDate { get; set; }

    public virtual ProductsT Product { get; set; } = null!;
}
