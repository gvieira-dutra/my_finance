using Microsoft.AspNetCore.Components;
using MudBlazor;
using MyFinance.Core.Handlers;
using MyFinance.Core.Requests.Categories;

namespace MyFinance.Web.Pages.Categories;

public partial class EditCategoryPage : ComponentBase
{
    #region Properties

    public bool IsBusy { get; set; }
    public UpdateCategoryRequest InputModel { get; set; } = new();

    #endregion

    #region Parameters
    [Parameter]
    public string CategoryId { get; set; } = null!;

    #endregion

    #region Services

    [Inject]
    public ICategoryHandler Handler { get; set; } = null!;

    [Inject]
    public NavigationManager NavigationManager { get; set; } = null!;

    [Inject]
    public ISnackbar Snackbar { get; set; } = null!;
    #endregion

    #region Override
    protected override async Task OnInitializedAsync()
    {
        GetCategoryByIdRequest? request = null!;

        try
        {
            request = new GetCategoryByIdRequest
            {
                Id = new Guid(CategoryId)
            };
        }
        catch
        {
            Snackbar.Add("Invalid parameter", Severity.Error);
        }

        if (request is null) return;

        IsBusy = true;
        try
        {
            var response = await Handler.GetByIdAsync(request);
            if (response is { IsSuccess: true, Data: not null })
                InputModel = new UpdateCategoryRequest
                {
                    Id = response.Data.Id,
                    Title = response.Data.Title,
                    Description = response.Data.Description
                };
        }
        catch
        {
            Snackbar.Add("Unable to edit category", Severity.Error);
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
            var result = await Handler.UpdateAsync(InputModel);

            if (result.IsSuccess)
            {
                Snackbar.Add("Category updated successfully", Severity.Success);

                NavigationManager.NavigateTo("/categories");
            }
        }
        catch
        {
            Snackbar.Add("Fail to update category", Severity.Error);
        }
        finally
        {
            IsBusy = false;
        }
    }
    #endregion
}
