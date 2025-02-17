namespace MyFinance.Core.Requests.Orders;

public class CreateOrderRequest : Request
{
    public Guid ProductId { get; set; }
    public long VoucherId { get; set; }
}
