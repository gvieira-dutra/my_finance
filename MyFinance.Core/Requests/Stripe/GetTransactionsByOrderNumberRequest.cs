namespace MyFinance.Core.Requests.Stripe;
public class GetTransactionsByOrderNumberRequest : Request
{
    public string Number { get; set; } = string.Empty;
}
