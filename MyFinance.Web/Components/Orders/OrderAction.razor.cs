using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using MyFinance.Core.Handlers;
using MyFinance.Core.Models;
using MyFinance.Core.Requests.Orders;
using MyFinance.Core.Requests.Stripe;
using MyFinance.Web.Pages.Orders;

namespace MyFinance.Web.Components.Orders;

public partial class OrderActionComponent : ComponentBase
{
    #region Parameters
    [CascadingParameter]
    public DetailsPage ParentPage { get; set; } = null!;

    [Parameter]
    [EditorRequired]
    public Order Order { get; set; } = null!;
    #endregion

    #region Services
    [Inject] public IDialogService DialogService { get; set; } = null!;
    [Inject] public IOrderHandler OrderHandler { get; set; } = null!;
    [Inject] public IStripeHandler StripeHandler { get; set; } = null!;
    [Inject] public ISnackbar Snackbar { get; set; } = null!;
    [Inject] public IJSRuntime JsRuntime { get; set; } = null!;

    #endregion


    #region Public Methods
    public async void OnCancelBtnClickedAsync()
    {
        bool? result = await DialogService.ShowMessageBox("ATENTION", "Are you sure you want to cancel this order? This action cannot be undone.", yesText: "YES", cancelText: "NO");

        if (result is not null && result == true)
            await CancelOrderAsync();
    }
    public async void OnRefundBtnClickedAsync()
    {
        bool? result = await DialogService.ShowMessageBox("ATENTION", "Are you sure you want to request a refund for this order? This action cannot be undone.", yesText: "YES", cancelText: "NO");

        if (result is not null && result == true)
            await RefundOrderAsync();
    }
    public async void OnPayBtnClickedAsync()
    {
        await PayOrderAsync();
    }

    #endregion

    #region Private Methods

    private async Task CancelOrderAsync()
    {
        var request = new CancelOrderRequest
        {
            Id = Order.Id
        };

        var result = await OrderHandler.CancelAsync(request);

        if (result.IsSuccess)
            ParentPage.RefreshState(result.Data!);
        else
            Snackbar.Add(result.Message ?? "", Severity.Error);
    }

    private async Task RefundOrderAsync()
    {
        var request = new RefundOrderRequest
        {
            Id = Order.Id
        };

        var result = await OrderHandler.RefundAsync(request);

        if (result.IsSuccess)
            ParentPage.RefreshState(result.Data!);
        else
            Snackbar.Add(result.Message ?? "", Severity.Error);
    }

    private async Task PayOrderAsync()
    {
        var request = new CreateSessionRequest
        {
            OrderNumber = Order.Number,
            OrderTotal = (int)Math.Round(Order.Total * 100, 2),
            ProductTitle = Order.Product.Title,
            ProductDescription = Order.Product.Description
        };

        try
        {
            var result = await StripeHandler.CreateSessionAsync(request);
            if (result.IsSuccess == false)
            {
                Snackbar.Add(result.Message ?? "", Severity.Error);
                return;
            }

            if (result.Data is null)
            {
                Snackbar.Add(result.Message ?? "", Severity.Error);
                return;
            }

            await JsRuntime.InvokeVoidAsync("checkout", Configuration.StripePublicKey, result.Data);
        }
        catch (Exception)
        {
            Snackbar.Add("Unable to start stripe session", Severity.Error);
        }
    }
    #endregion
}
