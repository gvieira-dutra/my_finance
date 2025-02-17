namespace MyFinance.Core.Requests.Categories;

public class GetCategoryByIdRequest : Request
{
    public Guid Id { get; set; }
}
