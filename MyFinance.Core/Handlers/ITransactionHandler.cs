using MyFinance.Core.Models;
using MyFinance.Core.Requests.Categories;
using MyFinance.Core.Requests.Transactions;
using MyFinance.Core.Response;

namespace MyFinance.Core.Handlers;

public interface ITransactionHandler
{
    Task<Response<Transaction?>> CreateAsync(CreateTransactionRequest request);
    Task<Response<Transaction?>> GetByIdAsync(GetTransactionByIdRequest request);
    Task<Response<Transaction?>> DeleteAsync(DeleteTransactionRequest request);
    Task<PagedResponse<List<Transaction>?>> GetByPeriodAsync(GetTransactionByPeriodRequest request);
    Task<Response<Transaction?>> UpdateAsync(UpdateTransactionRequest request);
}
