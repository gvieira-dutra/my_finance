namespace MyFinance.Core.Models;

public class Product : BaseClass
{
    public string Description { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public decimal Price { get; set; }
}
