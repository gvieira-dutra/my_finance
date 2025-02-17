using MyFinance.Core.Requests.Account;
using MyFinance.Core.Response;

namespace MyFinance.Core.Handlers;

public interface IAccountHandler
{
    Task<Response<string>> LoginAsync(LoginRequest request);
    Task<Response<string>> RegisterAsync(RegisterRequest request);
    Task LogoutAsync();
}
