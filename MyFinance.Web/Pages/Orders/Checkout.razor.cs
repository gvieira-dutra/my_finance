using Microsoft.AspNetCore.Components;
using MudBlazor;
using MyFinance.Core.Handlers;
using MyFinance.Core.Models;
using MyFinance.Core.Requests.Orders;

namespace MyFinance.Web.Pages.Orders;

public class CheckoutPage : ComponentBase
{
    #region Parameters
    [Parameter]
    public string ProductSlug { get; set; } = string.Empty;
    [SupplyParameterFromQuery(Name = "voucher")]
    public string? VoucherCode { get; set; }

    #endregion

    #region Properties
    public bool IsBusy { get; set; }
    public bool IsValid { get; set; }
    public CreateOrderRequest InputModel { get; set; } = new();
    public Product? Product { get; set; }
    public Voucher? Voucher { get; set; }
    public decimal Total { get; set; }
    public PatternMask VoucherMask = new("####-####")
    {
        MaskChars = [new MaskChar('#', "[0-9a-fA-F]")],
        Placeholder = '_',
        CleanDelimiters = true,
        Transformation = CharToUpper
    };

    #endregion

    #region Services
    [Inject]
    public IProductHandler ProductHandler { get; set; } = null!;
    [Inject]
    public IOrderHandler OrderHandler { get; set; } = null!;
    [Inject]
    public IVoucherHandler VoucherHandler { get; set; } = null!;
    [Inject]
    public NavigationManager Navigator { get; set; } = null!;
    [Inject]
    public ISnackbar Snackbar { get; set; } = null!;
    #endregion

    #region Override
    protected override async Task OnInitializedAsync()
    {
        try
        {
            var result = await ProductHandler.GetBySlugAsync(new GetProductBySlugRequest
            {
                Slug = ProductSlug
            });

            if (result.IsSuccess == false)
            {
                Snackbar.Add(result.Message ?? "", Severity.Error);
                IsValid = false;

                return;
            }

            Product = result.Data;
        }
        catch
        {
            Snackbar.Add("Unable to find product", Severity.Error);
            IsValid = false;
            return;
        }

        if (Product is null)
        {
            Snackbar.Add("Could not find requested product", Severity.Error);
            IsValid = false;

            return;
        }

        if (string.IsNullOrEmpty(VoucherCode) == false)
        {
            try
            {
                var result = await VoucherHandler.GetByNumberAsync(new GetVoucherByNumberRequest
                {
                    Number = VoucherCode.Replace("-", "")
                });

                if (result.IsSuccess == false || result.Data is null)
                {
                    VoucherCode = string.Empty;
                    Snackbar.Add("Unable to find voucher", Severity.Error);
                }

                Voucher = result.Data;
            }
            catch
            {
                Snackbar.Add("Unable to find voucher", Severity.Error);
                return;
            }
        }

        IsValid = true;
        Total = Product.Price - (Voucher?.Amount ?? 0);
    }
    #endregion

    #region Public Methods
    public async Task OnValidSubmitAsync()
    {
        IsBusy = true;
        try
        {
            var request = new CreateOrderRequest
            {
                ProductId = Product!.Id,
                VoucherId = Voucher?.Id ?? null
            };

            var result = await OrderHandler.CreateAsync(request);

            if (result.IsSuccess)
                Navigator.NavigateTo($"orders/{result.Data!.Number}");
            else
                Snackbar.Add(result.Message ?? "", Severity.Error);
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
        finally
        {
            IsBusy = false;
        }
    }

    public async Task GetCouponAsync()
    {
        if (!string.IsNullOrEmpty(VoucherCode))
        {
            var request = new GetVoucherByNumberRequest()
            {
                Number = VoucherCode ?? ""
            };

            var response = await VoucherHandler.GetByNumberAsync(request);

            if (!response.IsSuccess && response.Data is not null)
            {
                Snackbar.Add("The voucher code is not valid", Severity.Error);
                return;
            }
            else
            {
                Voucher = response.Data;
                Snackbar.Add("Voucher added successfully", Severity.Success);
                Total = Product!.Price - response.Data!.Amount;
                StateHasChanged();
                return;
            }
        }
    }
    #endregion

    #region Private Methods
    private static char CharToUpper(char c)
        => c.ToString().ToUpperInvariant()[0];

    #endregion

}
