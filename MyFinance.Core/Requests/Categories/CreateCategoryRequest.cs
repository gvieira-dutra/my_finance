using System.ComponentModel.DataAnnotations;

namespace MyFinance.Core.Requests.Categories;

public class CreateCategoryRequest : Request
{
    //Data Annotations
    [Required(ErrorMessage = "Title is required")]
    [MaxLength(80, ErrorMessage = "Title must be up to 80 char long")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Title is required")]
    public string Description { get; set; } = string.Empty;
}
