﻿namespace MyFinance.Core.Requests.Orders;

public class GetProductBySlugRequest : Request
{
    public string Slug { get; set; } = string.Empty;
}
