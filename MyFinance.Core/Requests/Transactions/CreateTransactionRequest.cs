using MyFinance.Core.Enum;
using System.ComponentModel.DataAnnotations;

namespace MyFinance.Core.Requests.Transactions;

public class CreateTransactionRequest : Request
{
    [Required(ErrorMessage ="Title is required")]
    [MaxLength(50, ErrorMessage ="Title too long")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Type is required")]
    public ETransactionType Type { get; set; } = ETransactionType.Withdraw;

    [Required(ErrorMessage = "Amount is required")]
    public decimal Amount { get; set; }

    [Required(ErrorMessage = "Category is required")]
    public Guid CategoryId { get; set; }

    [Required(ErrorMessage = "Date is required")]
    public DateTime? PaidOrReceivedAt { get; set; }
}
