using Microsoft.AspNetCore.Components;
using MudBlazor;
using MyFinance.Core.Handlers;
using MyFinance.Core.Models;
using MyFinance.Core.Requests.Categories;
using MyFinance.Core.Requests.Transactions;

namespace MyFinance.Web.Pages.Transactions;

public partial class CreateTransactionPage : ComponentBase
{
    #region Properties
    public bool IsBusy { get; set; } = false;
    public CreateTransactionRequest InputModel { get; set; } = new();
    public List<Category> Categories { get; set; } = [];

    #endregion

    #region Services

    [Inject]
    public ITransactionHandler TransactionHandler { get; set; } = null!;    [Inject]
    public ICategoryHandler CategoryHandler { get; set; } = null!;

    [Inject]
    public NavigationManager NavigationManager { get; set; } = null!;

    [Inject]
    public ISnackbar Snackbar { get; set; } = null!;
    #endregion

    #region Overrides

    protected override async Task OnInitializedAsync()
    {
        try
        {
            IsBusy = true;

            GetAllCategoriesRequest request = new();
            var result = await CategoryHandler.GetAllAsync(request);

            if (result.IsSuccess)
            {
                Categories = result.Data ?? [];
                InputModel.CategoryId = Categories.FirstOrDefault()?.Id ?? new Guid();
            }
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

    #endregion

    #region Methods

    public async Task OnValidSubmitAsync()
    {
        IsBusy = true;
        try
        {
            var result = await TransactionHandler.CreateAsync(InputModel);

            if (result.IsSuccess)
            {
                Snackbar.Add(result.Message ?? string.Empty, Severity.Success);
                NavigationManager.NavigateTo("/transactions/history");
            }
        }
        catch (Exception ex)   
        {
            Snackbar.Add(ex.Message ?? string.Empty, Severity.Error);
        }
        finally
        {
            IsBusy = false;
        }
    }

    #endregion
}
