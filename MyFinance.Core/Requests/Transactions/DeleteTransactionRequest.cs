namespace MyFinance.Core.Requests.Transactions
{
    public class DeleteTransactionRequest : Request
    {
        public Guid Id { get; set; }
    }
}
