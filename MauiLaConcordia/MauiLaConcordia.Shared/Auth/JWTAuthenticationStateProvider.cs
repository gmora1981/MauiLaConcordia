using MauiLaConcordia.Shared.Helpers;
using MauiLaConcordia.Shared.Model;
using MauiLaConcordia.Shared.Repository;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MauiLaConcordia.Shared.Auth
{
    public class JWTAuthenticationStateProvider : AuthenticationStateProvider, ILoginService
    {
        private readonly ITokenStorage tokenStorage;
        private readonly HttpClient httpClient;
        private readonly IAccountsRepository accountsRepository;
        private readonly string TOKENKEY = "TOKENKEY";
        private readonly string EXPIRATIONTOKENKEY = "EXPIRATIONTOKENKEY";

        private AuthenticationState Anonymous =>
            new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

        public JWTAuthenticationStateProvider(ITokenStorage tokenStorage, HttpClient httpClient,
            IAccountsRepository usersRepository)
        {
            this.tokenStorage = tokenStorage;
            this.httpClient = httpClient;
            this.accountsRepository = usersRepository;
        }

        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await tokenStorage.GetToken(TOKENKEY);

            if (string.IsNullOrEmpty(token))
            {
                return Anonymous;
            }

            var expirationTimeString = await tokenStorage.GetToken(EXPIRATIONTOKENKEY);
            DateTime expirationTime;

            if (DateTime.TryParse(expirationTimeString, out expirationTime))
            {
                if (IsTokenExpired(expirationTime))
                {
                    await CleanUp();
                    return Anonymous;
                }

                if (ShouldRenewToken(expirationTime))
                {
                    token = await RenewToken(token);
                }
            }

            return BuildAuthenticationState(token);
        }

        public async Task TryRenewToken()
        {
            var expirationTimeString = await tokenStorage.GetToken(EXPIRATIONTOKENKEY);
            DateTime expirationTime;

            if (DateTime.TryParse(expirationTimeString, out expirationTime))
            {
                if (IsTokenExpired(expirationTime))
                {
                    await Logout();
                }

                if (ShouldRenewToken(expirationTime))
                {
                    var token = await tokenStorage.GetToken(TOKENKEY);
                    var newToken = await RenewToken(token);
                    var authState = BuildAuthenticationState(newToken);
                    NotifyAuthenticationStateChanged(Task.FromResult(authState));
                }
            }
        }

        private async Task<string> RenewToken(string token)
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
            var newToken = await accountsRepository.RenewToken();
            await tokenStorage.SetToken(TOKENKEY, newToken.Token);
            await tokenStorage.SetToken(EXPIRATIONTOKENKEY, newToken.Expiration.ToString());
            return newToken.Token;
        }

        private bool ShouldRenewToken(DateTime expirationTime)
        {
            return expirationTime.Subtract(DateTime.UtcNow) < TimeSpan.FromMinutes(5);
        }

        private bool IsTokenExpired(DateTime expirationTime)
        {
            return expirationTime <= DateTime.UtcNow;
        }

        public AuthenticationState BuildAuthenticationState(string token)
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt")));
        }

        private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var claims = new List<Claim>();
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

            keyValuePairs.TryGetValue(ClaimTypes.Role, out object roles);

            if (roles != null)
            {
                if (roles.ToString().Trim().StartsWith("["))
                {
                    var parsedRoles = JsonSerializer.Deserialize<string[]>(roles.ToString());

                    foreach (var parsedRole in parsedRoles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, parsedRole));
                    }
                }
                else
                {
                    claims.Add(new Claim(ClaimTypes.Role, roles.ToString()));
                }

                keyValuePairs.Remove(ClaimTypes.Role);
            }

            claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString())));
            return claims;
        }

        private byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }

        public async Task Login(UserToken userToken)
        {
            await tokenStorage.SetToken(TOKENKEY, userToken.Token);
            await tokenStorage.SetToken(EXPIRATIONTOKENKEY, userToken.Expiration.ToString());
            var authState = BuildAuthenticationState(userToken.Token);
            NotifyAuthenticationStateChanged(Task.FromResult(authState));
        }

        public async Task Logout()
        {
            await CleanUp();
            NotifyAuthenticationStateChanged(Task.FromResult(Anonymous));
        }

        private async Task CleanUp()
        {
            await tokenStorage.RemoveToken(TOKENKEY);
            await tokenStorage.RemoveToken(EXPIRATIONTOKENKEY);
            httpClient.DefaultRequestHeaders.Authorization = null;
        }
    }
}