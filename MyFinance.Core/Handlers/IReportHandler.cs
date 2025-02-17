using MyFinance.Core.Models.Reports;
using MyFinance.Core.Requests.Reports;
using MyFinance.Core.Response;

namespace MyFinance.Core.Handlers;

public interface IReportHandler
{
    Task<Response<List<IncomeAndExpenses>?>> GetIncomesAndExpensesReportAsync(GetIncomesAnsExpensesRequest request);

    Task<Response<List<IncomeByCategory>?>> GetIncomesByCategoryReportAsync(GetIncomesByCategoryRequest request);

    Task<Response<List<ExpensesByCategory>?>> GetExpensesByCategoryReportAsync(GetExpenseByCategoryRequest request);

    Task<Response<FinancialSummary?>> GetFinancialSummaryReportAsync(GetFinancialSummaryRequest request);
}
