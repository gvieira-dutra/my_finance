using Microsoft.AspNetCore.Components;
using MudBlazor;
using MyFinance.Core.Handlers;
using MyFinance.Core.Requests.Categories;

namespace MyFinance.Web.Pages.Categories;

public class CreateCategoryPage : ComponentBase
{
    #region Properties
    public bool IsBusy { get; set; }
    public CreateCategoryRequest InputModel{ get; set; } = new();

    #endregion

    #region Services

    [Inject]
    public ICategoryHandler Handler { get; set; } = null!;
    
    [Inject]
    public NavigationManager NavigationManager { get; set; } = null!;

    [Inject]
    public ISnackbar Snackbar { get; set; } = null!;
    #endregion

    #region Methods

    public async Task OnValidSubmitAsync()
    {
        IsBusy = true;
        try
        {
            var result = await Handler.CreateAsync(InputModel);
            if (result.IsSuccess)
            {
                Snackbar.Add(result.Message ?? string.Empty, Severity.Success);
                NavigationManager.NavigateTo("/categories");
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
