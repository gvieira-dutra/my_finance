namespace MyFinance.Core.Models;

public abstract class BaseClass
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
}
