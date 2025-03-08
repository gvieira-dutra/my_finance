using Microsoft.AspNetCore.Components;
using MudBlazor;
using MyFinance.Core.Handlers;
using MyFinance.Core.Models;
using MyFinance.Core.Requests.Orders;

namespace MyFinance.Web.Pages.Orders;

public partial class ConfirmOrderPaymentPage : ComponentBase
{
    #region Parameters
    [Parameter]
    public string Number { get; set; } = string.Empty;
    #endregion

    #region Properties
    public Order? Order { get; set; }
    #endregion

    #region Services
    [Inject]
    public IOrderHandler OrderHandler { get; set; } = null!;
    [Inject]
    public ISnackbar Snackbar { get; set; } = null!;
    #endregion

    #region Overrides
    protected override async Task OnInitializedAsync()
    {
        var request = new PayOrderRequest
        {
            Number = Number
        };

        var result = await OrderHandler.PayAsync(request);

        if (result.IsSuccess == false)
        {
            Snackbar.Add(result.Message ?? "", Severity.Error);
            return;
        }

        Order = result.Data;
        Snackbar.Add(result.Message ?? "", Severity.Error);
    }
    #endregion
}
