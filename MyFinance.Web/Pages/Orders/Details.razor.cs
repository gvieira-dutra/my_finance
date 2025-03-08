using Microsoft.AspNetCore.Components;
using MudBlazor;
using MyFinance.Core.Handlers;
using MyFinance.Core.Models;
using MyFinance.Core.Requests.Orders;

namespace MyFinance.Web.Pages.Orders;

public partial class DetailsPage : ComponentBase
{
    #region Parameter
    [Parameter]
    public string OrderNumber { get; set; } = string.Empty;
    #endregion

    #region Properties
    public Order Order { get; set; } = null!;
    #endregion

    #region Services
    [Inject]
    public IOrderHandler Handler { get; set; } = null!;
    [Inject]
    public ISnackbar Snackbar { get; set; } = null!;
    #endregion

    #region Overrides
    protected override async Task OnInitializedAsync()
    {
        var request = new GetOrderByNumberRequest
        {
            Number = OrderNumber
        };

        var result = await Handler.GetByNumberAsync(request);

        if (result.IsSuccess)
        {
            Order = result.Data!;
        }
        else
            Snackbar.Add(result.Message ?? "", Severity.Error);
    }
    #endregion

    #region Methods
    public void RefreshState(Order order)
    {
        Order = order;
        StateHasChanged();
    }
    #endregion
}
