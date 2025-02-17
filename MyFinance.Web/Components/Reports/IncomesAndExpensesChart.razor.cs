using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Primitives;
using MudBlazor;
using MyFinance.Core.Handlers;
using MyFinance.Core.Requests.Reports;

namespace MyFinance.Web.Components.Reports;

public partial class IncomesAndExpensesChartComponent : ComponentBase
{
    #region Properties
    public ChartOptions Options { get; set; } = new();
    public List<ChartSeries>? Series { get; set; }
    //public List<string> Labels { get; set; } = null!;
    public string[]? Labels { get; set; } = Array.Empty<string>();
    #endregion

    #region Services
    [Inject]
    public IReportHandler Handler { get; set; } = null!;
    [Inject]
    public ISnackbar Snackbar { get; set; } = null!;
    #endregion

    #region Override
    protected override async Task OnInitializedAsync()
    {
        var request = new GetIncomesAnsExpensesRequest();
        var result = await Handler.GetIncomesAndExpensesReportAsync(request);

        if (!result.IsSuccess || result.Data is null)
        {
            Snackbar.Add("Unable to retrieve Income and Expenses report", Severity.Error);
            return;
        }

        var incomes = new List<double>();
        var expenses = new List<double>();
        var labels = new List<string>();

        foreach (var item in result.Data)
        {
            incomes.Add((double)item.Incomes);
            expenses.Add(-(double)item.Expenses);
            labels.Add(GetMonthName(item.Month));
        }

        Labels = labels.ToArray();

        Options.YAxisTicks = 1000;
        Options.LineStrokeWidth = 5;
        Options.ChartPalette = new[] { "#76FF01", Colors.Red.Default };

        Series = new List<ChartSeries>
                {
                    new ChartSeries{ Name = "Incomes", Data = incomes.ToArray() },
                    new ChartSeries{ Name = "Expenses", Data = expenses.ToArray() }
                };

        StateHasChanged();
    }

    private static string GetMonthName(int month)
        => new DateTime(DateTime.Now.Year, month, 1).ToString("MMMM");
    #endregion
}
