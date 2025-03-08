using Microsoft.AspNetCore.Components;
using MudBlazor;
using MyFinance.Core.Handlers;
using MyFinance.Core.Requests.Reports;

namespace MyFinance.Web.Components.Reports;

public partial class ExpensesByCategoryChartComponent : ComponentBase
{
    #region Properties 
    public List<double> Data { get; set; } = [];
    public List<string> Labels { get; set; } = [];

    #endregion

    #region Services
    [Inject]
    public IReportHandler Handler { get; set; } = null!;
    [Inject]
    public ISnackbar SnackBar { get; set; } = null!;
    #endregion

    #region Override
    protected override async Task OnInitializedAsync()
    {
        await GetExpensesByCategoryAsync();
    }

    private async Task GetExpensesByCategoryAsync()
    {
        var request = new GetExpenseByCategoryRequest();
        var result = await Handler.GetExpensesByCategoryReportAsync(request);

        if (!result.IsSuccess || result.Data is null)
        {
            SnackBar.Add("Fail to retrieve Expense by Category Data");
            return;
        }

        foreach (var item in result.Data)
        {
            Labels.Add($"{item.Category} ({item.Expenses:C})");
            Data.Add((double)item.Expenses);
        }

    }
    #endregion
}
