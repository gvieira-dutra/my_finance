using Microsoft.AspNetCore.Components;
using MudBlazor;
using MyFinance.Core.Handlers;
using MyFinance.Core.Requests.Account;
using MyFinance.Web.Security;

namespace MyFinance.Web.Pages.Identity
{
    public partial class LoginPage : ComponentBase
    {

        #region Dependencies

        [Inject]
        public ISnackbar Snackbar { get; set; } = null!;
        [Inject]
        public IAccountHandler Handler { get; set; } = null!;

        [Inject]
        public NavigationManager Navigation{ get; set; } = null!;

        [Inject]
        public ICookieAuthenticationStateProvider AuthenticationStateProvider { get; set; } = null!;

        #endregion

        #region Properties

        public LoginRequest InputModel { get; set; } = new();
        public bool IsBusy { get; set; } = false;
        #endregion


        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            //Merge Pattern
            if (user.Identity is { IsAuthenticated: true })
                Navigation.NavigateTo("/");


        }
        #endregion

        #region Methods

        public async Task OnValidSubmitAsync()
        {
            IsBusy = true;

            try
            {
                var result = await Handler.LoginAsync(InputModel);

                if (result.IsSuccess)
                {
                    await AuthenticationStateProvider.GetAuthenticationStateAsync();
                    AuthenticationStateProvider.AuthenticationStateChangedNotifier();
                    Navigation.NavigateTo("/");
                }
                else
                    Snackbar.Add(result.Message ?? string.Empty, Severity.Error);
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
}
