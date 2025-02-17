using Microsoft.AspNetCore.Components;
using MudBlazor;
using MyFinance.Core.Handlers;
using MyFinance.Core.Models;
using MyFinance.Core.Requests.Categories;

namespace MyFinance.Web.Pages.Categories;

public partial class ListCategoryPage : ComponentBase
{
    #region Properties
    public bool IsBusy { get; set; }
    public List<Category> Categories { get; set; } = [];
    public string SearchTerm { get; set; } = string.Empty;
    #endregion

    #region Services
    [Inject]
    public ISnackbar Snackbar { get; set; } = null!;

    [Inject]
    public IDialogService DialogService { get; set; } = null!;
    [Inject]
    public ICategoryHandler Handler { get; set; } = null!;

    #endregion

    #region Override
    protected override async Task OnInitializedAsync()
    {
        IsBusy = true;

        try
        {
            var request = new GetAllCategoriesRequest();
            var result = await Handler.GetAllAsync(request);

            if (result.IsSuccess)
            {
                Categories = result.Data ?? []; 
            }
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message ?? string.Empty, Severity.Error);
            
            Snackbar.Add("Hit here", Severity.Error);
        }
        finally
        {
            IsBusy = false;
        }

    }
    #endregion

    #region Methods
    public Func<Category, bool> Filter => category =>
    {
        if (string.IsNullOrEmpty(SearchTerm))
        {
            return true;
        }

        if (category.Title.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase))
            return true;

        if (!string.IsNullOrEmpty(category.Description) 
            && category.Description.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase))
            return true;

        return false;
    };
    
    public async void OnDeleteButtonClicked(Guid id, string title)
    {
        var result = await DialogService.ShowMessageBox("ATTENTION", $"Your are about to delete category: {title}. \n This action can't be undone. Do you want to proceed?", yesText: "Delete", noText: "Cancel");

        if (result is true)
            await DeleteAsync(id, title);
          
        StateHasChanged();
    }

    public async Task DeleteAsync(Guid id, string title)
    {
        try
        {
            await Handler.DeleteAsync(new 
                    DeleteCategoryRequest { Id = id });

            Categories.RemoveAll(x => x.Id == id);

            Snackbar.Add($"Category: {title} deleted successfully.", Severity.Success);
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
    }

    #endregion

}
