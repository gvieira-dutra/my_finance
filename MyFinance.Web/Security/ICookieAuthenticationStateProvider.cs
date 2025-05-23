﻿using Microsoft.AspNetCore.Components.Authorization;

namespace MyFinance.Web.Security;

public interface ICookieAuthenticationStateProvider
{
    Task<bool> CheckAuthenticatedAsync();
    Task<AuthenticationState> GetAuthenticationStateAsync();
    void AuthenticationStateChangedNotifier();
}

