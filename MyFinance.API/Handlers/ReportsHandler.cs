using Microsoft.EntityFrameworkCore;
using MyFinance.API.Data;
using MyFinance.Core.Enum;
using MyFinance.Core.Handlers;
using MyFinance.Core.Models.Reports;
using MyFinance.Core.Requests.Reports;
using MyFinance.Core.Response;

namespace MyFinance.API.Handlers;

public class ReportsHandler(AppDbContext context) : IReportHandler
{
    public async Task<Response<List<ExpensesByCategory>?>> GetExpensesByCategoryReportAsync(GetExpenseByCategoryRequest request)
    {
        try
        {
            var expensesByCategory = await context.Transactions
            .Where(t => t.PaidOrReceivedAt.HasValue && // Ensure it's not null
                t.PaidOrReceivedAt.Value >= DateTime.UtcNow.AddMonths(-11) &&
                t.PaidOrReceivedAt.Value < DateTime.UtcNow.AddMonths(1) &&
                t.Type == ETransactionType.Withdraw)
            .GroupBy(t => new
            {
                t.UserId,
                t.Category.Title,
                Year = t.PaidOrReceivedAt.Value.Year,
            })
            .Select(g => new ExpensesByCategory
            (
                g.Key.UserId,
                g.Key.Title,
                g.Key.Year,
                g.Sum(t => t.Amount)

            ))
            .ToListAsync();

            //    var data = await context.ExpensesByCategory
            //.AsNoTracking()
            //.Where(x => x.UserId == request.UserId)
            //.OrderByDescending(x => x.Year)
            //.ThenBy(x => x.Category)
            //.ToListAsync();

            return new Response<List<ExpensesByCategory>?>(expensesByCategory);

        }
        catch
        {
            return new Response<List<ExpensesByCategory>?>(null, 500, "[API024] Unable to retrieve Expenses by Category report");
        }
    }

    public async Task<Response<FinancialSummary?>> GetFinancialSummaryReportAsync(GetFinancialSummaryRequest request)
    {
        //This method will retrieve current month's financial summary. 
        var startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        var endDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1)
            .AddMonths(1)
            .AddDays(-1);

        try
        {
            var data = await context
        .Transactions
        .AsNoTracking()
        .Where(x => x.UserId == request.UserId && x.PaidOrReceivedAt >= startDate && x.PaidOrReceivedAt <= endDate)
        .GroupBy(x => 1)
        .Select(x => new FinancialSummary(
            request.UserId,
            x.Where(ty => ty.Type == ETransactionType.Deposit).Sum(t => t.Amount),
            x.Where(ty => ty.Type == ETransactionType.Withdraw).Sum(t => t.Amount)
            ))
        .FirstOrDefaultAsync();

            return new Response<FinancialSummary?>(data);
        }
        catch
        {
            return new Response<FinancialSummary?>(null, 500, "[API025] Unable to retrieve Financial Summary report");
        }
    }

    public async Task<Response<List<IncomeAndExpenses>?>> GetIncomesAndExpensesReportAsync(GetIncomesAnsExpensesRequest request)
    {
        try
        {

            var incomesAndExpenses = await context.Transactions
            .Where(t => t.PaidOrReceivedAt.HasValue &&
                t.PaidOrReceivedAt.Value >= DateTime.UtcNow.AddMonths(-11) &&
                t.PaidOrReceivedAt.Value < DateTime.UtcNow.AddMonths(1))
            .GroupBy(t => new
            {
                t.UserId,
                Month = t.PaidOrReceivedAt.Value.Month,
                Year = t.PaidOrReceivedAt.Value.Year
            })
            .Select(g => new IncomeAndExpenses
            (
                g.Key.UserId,
                g.Key.Month,
                g.Key.Year,
                g.Sum(t => t.Type == ETransactionType.Deposit ? t.Amount : 0),
                g.Sum(t => t.Type == ETransactionType.Withdraw ? t.Amount : 0)
            ))
            .ToListAsync();


            //    var data = await context.IncomesAndExpenses
            //.AsNoTracking()
            //.Where(x => x.UserId == request.UserId)
            //.OrderByDescending(x => x.Year)
            //.ThenBy(x => x.Month)
            //.ToListAsync();

            return new Response<List<IncomeAndExpenses>?>(incomesAndExpenses);
        }
        catch
        {
            return new Response<List<IncomeAndExpenses>?>(null, 500, "[API022] Unable to retrieve Income and Expenses report");
        }
    }

    public async Task<Response<List<IncomeByCategory>?>> GetIncomesByCategoryReportAsync(GetIncomesByCategoryRequest request)
    {
        try
        {

            var incomesByCategory = await context.Transactions
            .Where(t => t.PaidOrReceivedAt.HasValue &&
                t.PaidOrReceivedAt.Value >= DateTime.UtcNow.AddMonths(-11) &&
                t.PaidOrReceivedAt.Value < DateTime.UtcNow.AddMonths(1) &&
                t.Type == ETransactionType.Deposit)
            .Join(context.Categories,
                  t => t.CategoryId,
                  c => c.Id,
                  (t, c) => new
                  {
                      t.UserId,
                      Category = c.Title,
                      Year = t.PaidOrReceivedAt.Value.Year,
                      Amount = t.Amount
                  })
            .GroupBy(g => new { g.UserId, g.Category, g.Year })
            .Select(g => new IncomeByCategory
            (
                g.Key.UserId,
                g.Key.Category,
                g.Key.Year,
                g.Sum(x => x.Amount)
            ))
            .ToListAsync();


            //    var data = await context.IncomesByCategory
            //.AsNoTracking()
            //.Where(x => x.UserId == request.UserId)
            //.OrderByDescending(x => x.Year)
            //.ThenBy(x => x.Category)
            //.ToListAsync();

            return new Response<List<IncomeByCategory>?>(incomesByCategory);
        }
        catch
        {
            return new Response<List<IncomeByCategory>?>(null, 500, "[API023] Unable to retrieve Income by Category report");
        }
    }

}
