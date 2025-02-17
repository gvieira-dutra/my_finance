using Microsoft.AspNetCore.Components;
using MudBlazor;
using MyFinance.Core.Handlers;
using MyFinance.Core.Models.Reports;
using MyFinance.Core.Requests.Reports;

namespace MyFinance.Web.Pages;

public partial class HomePage : ComponentBase
{
    #region Properties
    public bool ShowValues { get; set; } = true;
    public FinancialSummary? Summary { get; set; }
    #endregion

    #region Services
    [Inject]
    public ISnackbar Snackbar { get; set; } = null!;
    [Inject]
    public IReportHandler Handler { get; set; } = null!;
    #endregion

    #region Override
    protected override async Task OnInitializedAsync()
    {
        var request = new GetFinancialSummaryRequest();
        var response = await Handler.GetFinancialSummaryReportAsync(request);

        if (response.IsSuccess)
            Summary = response.Data;
    }
    #endregion

    #region Methods
    public void ToggleShowValues() => ShowValues = !ShowValues;
    #endregion
}
