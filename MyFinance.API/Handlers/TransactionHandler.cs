using Microsoft.EntityFrameworkCore;
using MyFinance.API.Data;
using MyFinance.Core.Common.Extensions;
using MyFinance.Core.Enum;
using MyFinance.Core.Handlers;
using MyFinance.Core.Models;
using MyFinance.Core.Requests.Categories;
using MyFinance.Core.Requests.Transactions;
using MyFinance.Core.Response;

namespace MyFinance.API.Handlers
{
    public class TransactionHandler(AppDbContext context) : ITransactionHandler
    {
        public async Task<Response<Transaction?>> CreateAsync(CreateTransactionRequest request)
        {
            if (request is { Type: ETransactionType.Withdraw, Amount: > 0 })
                request.Amount *= -1;
            try
            {
                var transaction = new Transaction
                {
                    Amount = request.Amount,
                    CategoryId = request.CategoryId,
                    CreatedAt = DateTime.Now,
                    PaidOrReceivedAt = request.PaidOrReceivedAt,
                    Title = request.Title,
                    Type = request.Type,
                    UserId = request.UserId
                };

                await context.Transactions.AddAsync(transaction);
                await context.SaveChangesAsync();

                return new Response<Transaction?>(transaction, 201, "[API012] Transaction created successfully");
            }
            catch
            {
                return new Response<Transaction?>(null, 500, "Unable to create transaction");
            }
        }

        public async Task<Response<Transaction?>> DeleteAsync(DeleteTransactionRequest request)
        {
            try
            {
                var transaction = await context
                    .Transactions
                    .FirstOrDefaultAsync(x => x.Id == request.Id);

                if (transaction is null)
                    return new Response<Transaction?>(null, 404, "[API016] Transaction not found");

                context.Transactions.Remove(transaction);
                await context.SaveChangesAsync();

                return new Response<Transaction?>(transaction);
            }
            catch
            {
                return new Response<Transaction?>(null, 500, "[API018] Unable to delete transaction");
            }
        }

        public async Task<Response<Transaction?>> GetByIdAsync(GetTransactionByIdRequest request)
        {
            try
            {
                var transaction = await context
                       .Transactions
                       .FirstOrDefaultAsync(x => x.Id == request.Id);

                return transaction is null
                    ? new Response<Transaction?>(null, 404, "[API019] Unable to find transaction")
                    : new Response<Transaction?>(transaction);
            }
            catch 
            {
                return new Response<Transaction?>(null, 500, "[API020] Unable to find transaction");
            }

        }

        public async Task<PagedResponse<List<Transaction>?>> GetByPeriodAsync(GetTransactionByPeriodRequest request)
        {
            try
            {
                request.StartDate = request.StartDate ?? DateTime.Now.GetFirstDay();
                request.EndDate = request.EndDate ?? DateTime.Now.GetLastDay();
            }
            catch
            {
                return new PagedResponse<List<Transaction>?>(null, 500, "[API021] Failed to retrieve dates");
            }

            var query = context
                .Transactions
                .AsNoTracking()
                .Where(x => x.PaidOrReceivedAt >= request.StartDate
                && x.PaidOrReceivedAt <= request.EndDate
                && x.UserId == request.UserId)
                .OrderBy(x => x.CreatedAt);

            var transactions = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            return new PagedResponse<List<Transaction>?>(transactions);
        }

        public async Task<Response<Transaction?>> UpdateAsync(UpdateTransactionRequest request)
        {
            if (request is { Type: ETransactionType.Withdraw, Amount: > 0 })
                request.Amount *= -1;
            try
            {
                var transaction = await context.Transactions.FirstOrDefaultAsync(x => x.Id == request.Id);

                if (transaction is null)
                    return new Response<Transaction?>(null, 404, "[API013] Transaction not found");

                transaction.CategoryId = request.CategoryId;
                transaction.Amount = request.Amount;
                transaction.Title = request.Title;
                transaction.Type = request.Type;
                transaction.PaidOrReceivedAt = request.PaidOrReceivedAt;

                context.Transactions.Update(transaction);
                await context.SaveChangesAsync();

                return new Response<Transaction?>(transaction);
            }
            catch
            {
                return new Response<Transaction?>(null, 500, "[API015] Unable to update transaction");
            }
        }

    }
}
