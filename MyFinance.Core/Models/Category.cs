namespace MyFinance.Core.Models;

public class Category : BaseClass
{
    public string? Description { get; set; }
    public string UserId { get; set; } = string.Empty;
}
