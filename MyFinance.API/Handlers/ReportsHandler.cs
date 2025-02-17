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
            var data = await context.ExpensesByCategory
        .AsNoTracking()
        .Where(x => x.UserId == request.UserId)
        .OrderByDescending(x => x.Year)
        .ThenBy(x => x.Category)
        .ToListAsync();

            return new Response<List<ExpensesByCategory>?>(data);

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
            var data = await context.IncomesAndExpenses
        .AsNoTracking()
        .Where(x => x.UserId == request.UserId)
        .OrderByDescending(x => x.Year)
        .ThenBy(x => x.Month)
        .ToListAsync();

            return new Response<List<IncomeAndExpenses>?>(data);
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
            var data = await context.IncomesByCategory
        .AsNoTracking()
        .Where(x => x.UserId == request.UserId)
        .OrderByDescending(x => x.Year)
        .ThenBy(x => x.Category)
        .ToListAsync();

            return new Response<List<IncomeByCategory>?>(data);
        }
        catch
        {
            return new Response<List<IncomeByCategory>?>(null, 500, "[API023] Unable to retrieve Income by Category report");
        }
    }
}
