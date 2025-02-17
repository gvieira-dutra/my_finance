using Microsoft.AspNetCore.Components;
using MudBlazor;
using MyFinance.Core.Handlers;
using MyFinance.Core.Models;
using MyFinance.Core.Requests.Categories;
using MyFinance.Core.Requests.Transactions;
using System.Runtime.CompilerServices;

namespace MyFinance.Web.Pages.Transactions;

public partial class EditTransactionPage : ComponentBase
{
    #region Properties
    [Parameter]
    public string TransactionId { get; set; } = "";
    public bool IsBusy { get; set; } = false; 
    public UpdateTransactionRequest InputModel { get; set; } = new();
    public List<Category> Categories { get; set; } = [];

    #endregion

    #region Services

    [Inject]
    public ITransactionHandler TransactionHandler { get; set; } = null!; [Inject]
    public ICategoryHandler CategoryHandler { get; set; } = null!;

    [Inject]
    public NavigationManager NavigationManager { get; set; } = null!;

    [Inject]
    public ISnackbar Snackbar { get; set; } = null!;
    #endregion

    #region Overrides

    protected override async Task OnInitializedAsync()
    {
        IsBusy = true;

        await GetTransactionByIdAsync();
        await GetCategoriesAsync();

        IsBusy = false;
    }

    #endregion

    #region Private Methods
     
    private async Task GetTransactionByIdAsync()
    {

            IsBusy = true;
        try
        {
            var request = new GetTransactionByIdRequest { Id = new Guid(TransactionId) };

            var result = await TransactionHandler.GetByIdAsync(request);

            if (result.IsSuccess && result.Data is not null)
            {
                InputModel = new UpdateTransactionRequest 
                { 
                    CategoryId = result.Data.CategoryId,
                    PaidOrReceivedAt = result.Data.PaidOrReceivedAt,
                    Title = result.Data.Title,
                    Type = result.Data.Type,
                    Amount = result.Data.Amount,
                    Id = result.Data.Id
                };

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

    private async Task GetCategoriesAsync()
    {

            IsBusy = true;
        try
        {
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
            var result = await TransactionHandler.UpdateAsync(InputModel);

            if (result.IsSuccess)
            {
                Snackbar.Add("Transaction Updated", Severity.Success);
                NavigationManager.NavigateTo("/transactions/history");
            }
            else
            {
                Snackbar.Add(result.Message ?? "Error updating transaction", Severity.Error);
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
}
