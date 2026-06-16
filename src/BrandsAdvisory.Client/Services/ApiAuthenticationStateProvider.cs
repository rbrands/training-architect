using BrandsAdvisory.Client.Models;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;
using System.Security.Claims;

namespace BrandsAdvisory.Client.Services;

/// <summary>
/// Provides authentication state in the WebAssembly client by calling
/// <c>/api/user</c> on the host server. The server reads the authentication
/// cookie and returns the user's identity and roles.
/// </summary>
public class ApiAuthenticationStateProvider(HttpClient http) : AuthenticationStateProvider
{
    private static readonly AuthenticationState AnonymousState =
        new(new ClaimsPrincipal(new ClaimsIdentity()));

    private AuthenticationState? _cached;

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (_cached is not null)
            return _cached;

        try
        {
            var info = await http.GetFromJsonAsync<UserInfo>("/api/user");
            if (info?.IsAuthenticated == true)
            {
                var claims = new List<Claim> { new(ClaimTypes.Name, info.Name) };
                claims.AddRange(info.Roles.Select(r => new Claim(ClaimTypes.Role, r)));
                var identity = new ClaimsIdentity(claims, authenticationType: "Cookie");
                _cached = new AuthenticationState(new ClaimsPrincipal(identity));
            }
            else
            {
                _cached = AnonymousState;
            }
        }
        catch
        {
            _cached = AnonymousState;
        }

        return _cached;
    }
}
