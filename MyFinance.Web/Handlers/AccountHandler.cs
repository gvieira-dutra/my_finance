using System.Net.Http.Json;
using System.Text;
using MyFinance.Core.Handlers;
using MyFinance.Core.Requests.Account;
using MyFinance.Core.Response;

namespace MyFinance.Web.Handlers;

public class AccountHandler(IHttpClientFactory httpClientFactory) : IAccountHandler
{
    private readonly HttpClient _client = httpClientFactory.CreateClient(
        Configuration.HttpClientName
    );

    public async Task<Response<string>> LoginAsync(LoginRequest request)
    {
        var result = await _client.PostAsJsonAsync("v1/identity/login?useCookies=true", request);

        return result.IsSuccessStatusCode
            ? new Response<string>("Login successful", (int)result.StatusCode, "Login successful")
            : new Response<string>(
                null,
                (int)result.StatusCode,
                "Unable possible to log in. Try again."
            );
    }

    public async Task LogoutAsync()
    {
        var emptyContent = new StringContent("{}", Encoding.UTF8, "application/json");

        await _client.PostAsJsonAsync("v1/identity/logout", emptyContent);
    }

    public async Task<Response<string>> RegisterAsync(RegisterRequest request)
    {
        var result = await _client.PostAsJsonAsync("v1/identity/register", request);

        return result.IsSuccessStatusCode
            ? new Response<string>(
                "Account registration successful",
                (int)result.StatusCode,
                "Account registration successful"
            )
            : new Response<string>(
                null,
                (int)result.StatusCode,
                "Unable to register account. Try again."
            );
    }
}
