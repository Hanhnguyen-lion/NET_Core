using System;
using System.Collections.Generic;

namespace MyAPI;

public partial class ProductsT
{
    public int ProductId { get; set; }

    public string ProductName { get; set; } = null!;

    public string? Description { get; set; }

    public int? Quantity { get; set; }

    public decimal? Price { get; set; }

    public decimal? Total { get; set; }

    public DateTime OrderDate { get; set; }

    public DateTime CreateDate { get; set; }

    public DateTime ModifyDate { get; set; }

    public virtual ICollection<OrdersT> OrdersTs { get; set; } = new List<OrdersT>();
}
