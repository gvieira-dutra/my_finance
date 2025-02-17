using MyFinance.Core.Handlers;
using MyFinance.Core.Models.Reports;
using MyFinance.Core.Requests.Reports;
using MyFinance.Core.Response;
using System.Net.Http.Json;

namespace MyFinance.Web.Handlers
{
    public class ReportHandler(IHttpClientFactory httpClientFactory) : IReportHandler
    {
        private readonly HttpClient _client = httpClientFactory.CreateClient(Configuration.HttpClientName);

        public async Task<Response<List<ExpensesByCategory>?>> GetExpensesByCategoryReportAsync(GetExpenseByCategoryRequest request)
        {
            return await _client.GetFromJsonAsync<Response<List<ExpensesByCategory>?>>($"v1/reports/expenses")
                ?? new Response<List<ExpensesByCategory>?>(null, 500, "Unable to retrieve Expenses by Category data.");
        }

        public async Task<Response<FinancialSummary?>> GetFinancialSummaryReportAsync(GetFinancialSummaryRequest request)
        {
            return await _client.GetFromJsonAsync<Response<FinancialSummary?>>($"v1/reports/summary")
                ?? new Response<FinancialSummary?>(null, 500, "Unable to retrieve Financial Summary data.");
        }

        public async Task<Response<List<IncomeAndExpenses>?>> GetIncomesAndExpensesReportAsync(GetIncomesAnsExpensesRequest request)
        {
            return await _client.GetFromJsonAsync<Response<List<IncomeAndExpenses>?>>($"v1/reports/incomes-expenses")
                ?? new Response<List<IncomeAndExpenses>?>(null, 500, "Unable to retrieve Income and Expenses data.");
        }

        public async Task<Response<List<IncomeByCategory>?>> GetIncomesByCategoryReportAsync(GetIncomesByCategoryRequest request)
        {
            return await _client.GetFromJsonAsync<Response<List<IncomeByCategory>?>>($"v1/reports/incomes")
               ?? new Response<List<IncomeByCategory>?>(null, 500, "Unable to retrieve Income by Category data.");
        }
    }
}
