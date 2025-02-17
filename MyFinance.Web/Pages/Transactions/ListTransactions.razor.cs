using Microsoft.AspNetCore.Components;
using MudBlazor;
using MyFinance.Core.Common.Extensions;
using MyFinance.Core.Handlers;
using MyFinance.Core.Models;
using MyFinance.Core.Requests.Transactions;

namespace MyFinance.Web.Pages.Transactions
{
    public partial class ListTransactionsPage : ComponentBase
    {
        #region Properties
        public bool IsBusy { get; set; } = false;
        public List<Transaction> Transactions { get; set; } = [];
        public string SearchTerm { get; set; } = "";
        public int CurrentYear { get; set; } = DateTime.Now.Year;
        public int CurrentMonth { get; set; } = DateTime.Now.Month;
        public int[] Years { get; set; } =
        {
            DateTime.Now.AddYears(1).Year,
            DateTime.Now.Year,
            DateTime.Now.AddYears(-1).Year,
            DateTime.Now.AddYears(-2).Year,
            DateTime.Now.AddYears(-3).Year,
            DateTime.Now.AddYears(-4).Year,
            DateTime.Now.AddYears(-5).Year,
            DateTime.Now.AddYears(-6).Year,
            DateTime.Now.AddYears(-7).Year,
            DateTime.Now.AddYears(-8).Year,
            DateTime.Now.AddYears(-9).Year,
            DateTime.Now.AddYears(-10).Year,

        };
        #endregion

        #region Services
        [Inject]
        public ISnackbar Snackbar { get; set; } = null!;

        [Inject]
        public IDialogService DialogService { get; set; } = null!;

        [Inject]
        public ITransactionHandler Handler { get; set; } = null!;
        #endregion

        #region Private Methods
        private async Task GetTransactionAsync()
        {
                IsBusy = true;
            try
            {

                var request = new GetTransactionByPeriodRequest
                {
                    StartDate = DateTime.Now.GetFirstDay(CurrentYear, CurrentMonth),
                    EndDate = DateTime.Now.GetLastDay(CurrentYear, CurrentMonth),
                    PageNumber = 1,
                    PageSize = 1000
                };
                

                var result = await Handler.GetByPeriodAsync(request);

                if (result.IsSuccess)
                    Transactions = result.Data ?? [];
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
       
        private async Task OnDeleteAsync(Guid id, string title)
        {
            IsBusy = true;
            try
            {
                var result = await Handler.DeleteAsync(new DeleteTransactionRequest { Id = id});

                if (result.IsSuccess)
                {
                    Snackbar.Add($"Transaction: {title} deleted!", Severity.Success);
                    Transactions.RemoveAll(x => x.Id == id);
                }
                else
                {
                    Snackbar.Add(result.Message ?? "", Severity.Error);
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

        #region Override
        protected override async Task OnInitializedAsync()
            => await GetTransactionAsync();

        #endregion

        #region Public Methods
        public Func<Transaction, bool> Filter => transaction =>
        {
            if (string.IsNullOrEmpty(SearchTerm))
                return true;

            return transaction.Id.ToString().Contains(SearchTerm, StringComparison.OrdinalIgnoreCase)
                    || transaction.Title.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase);
        }; 

        public async void OnDeleteButtonClickedAsync(Guid id, string title)
        {
            var result = await DialogService.ShowMessageBox(
            "ATTENTION", $"You are about to delete the transaction: {title}. This action can't be undone. Continue?", yesText: "DELETE", noText: "Cancel");

            if (result is true)
                await OnDeleteAsync(id, title);

            StateHasChanged();
        }
        #endregion

        public async Task OnSearchAsync()
        {
            await GetTransactionAsync();
            StateHasChanged();
        }
    }
}
