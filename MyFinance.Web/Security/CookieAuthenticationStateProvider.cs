﻿using Microsoft.AspNetCore.Components.Authorization;
using MyFinance.API.Endpoints.Identity;
using MyFinance.Core.Models.Account;
using System.Linq.Expressions;
using System.Net.Http.Json;
using System.Security.Claims;

namespace MyFinance.Web.Security;

public class CookieAuthenticationStateProvider(IHttpClientFactory httpClientFactory) : AuthenticationStateProvider, ICookieAuthenticationStateProvider
{
    private bool _isAuthenticated = false;
    private readonly HttpClient _client = httpClientFactory.CreateClient(Configuration.HttpClientName);
    public async Task<bool> CheckAuthenticatedAsync()
    {
        await GetAuthenticationStateAsync();
        return _isAuthenticated;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        _isAuthenticated = false;

        var user = new ClaimsPrincipal(new ClaimsIdentity());

        var userInfo = await GetUserAsync();
        if (userInfo is null) 
            return new AuthenticationState(user);

        var claims = await GetClaimsAsync(userInfo);

        var id = new ClaimsIdentity(claims, nameof(CookieAuthenticationStateProvider));
        user = new ClaimsPrincipal(id);

        _isAuthenticated = true;
        return new AuthenticationState(user);
    }

    public void AuthenticationStateChangedNotifier()
        => NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());

    private async Task<User?> GetUserAsync()
    {
        try
        {
            return await _client.GetFromJsonAsync<User?>("v1/identity/manage/info");
        }
        catch 
        {
            return null;
        }
    }

    private async Task<List<Claim>> GetClaimsAsync(User user)
    {
        var claims = new List<Claim>()
        {
            new (ClaimTypes.Name, user.Email),
            new (ClaimTypes.Email, user.Email)
        };

        claims.AddRange(
        user.Claims.Where(x =>
        x.Key != ClaimTypes.Name &&
        x.Key != ClaimTypes.Email)
        .Select(x => new Claim(x.Key, x.Value)));

        RoleClaim[]? roles;

        try
        {
            roles = await _client.GetFromJsonAsync<RoleClaim[]>("v1/identity/roles");
        }
        catch
        {
            return claims;
        }

        foreach (var role in roles ?? [])
        {
            if (!string.IsNullOrEmpty(role.Type) && !string.IsNullOrEmpty(role.Value))
                claims.Add(new Claim(role.Type, role.Value, role.ValueType, role.Issuer, role.OriginalIssuer));
        }

        return claims;
    }
}
