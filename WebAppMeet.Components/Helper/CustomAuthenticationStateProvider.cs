using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace WebAppMeet.Components.Helper
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        string _token { get; set; }
        string _authType { get; set; }
        public CustomAuthenticationStateProvider()
        {
            _authType = "jwt";
        } public CustomAuthenticationStateProvider(string authType)
        {
            _authType = authType;
        }
        public async Task SetToken(string token, string authType = null)
        {
            _token = token;

            if (authType != null)
                _authType = authType;

            await Task.CompletedTask;
        }
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            ClaimsIdentity identity = null;
            ClaimsPrincipal user = null;
            AuthenticationState state = null;


            if (_token != null)
            {
                identity = new ClaimsIdentity(ParseJWTClaims(_token), _authType);
                user = new ClaimsPrincipal(identity);
                state = new AuthenticationState(user);
            }
            else
            {
                identity = new ClaimsIdentity(new List<Claim>(), _authType);
                user = new ClaimsPrincipal(identity);
                state = new AuthenticationState(user);

            }

            NotifyAuthenticationStateChanged(Task.FromResult(state));
            return await Task.FromResult(state);
        }

        public async Task<AuthenticationState> GetAuthenticationStateAsync(string authType = null, string token = null)
        {
            ClaimsIdentity identity = null;
            ClaimsPrincipal user = null;
            AuthenticationState state = null;


            if (token != null)
            {
                var claims = ParseJWTClaims(token).ToList();
                claims.Add(new Claim(ClaimTypes.Name, claims.First(x=>x.Type=="email").Value));
                var id = new ClaimsIdentity(claims, claims.First(x => x.Type == "email").Value);
                identity = new ClaimsIdentity(claims, authType);
                identity.Actor = id;
                user = new ClaimsPrincipal(identity);
                state = new AuthenticationState(user);
            }
            else
            {
                identity = new ClaimsIdentity(new List<Claim>(), authType);
                user = new ClaimsPrincipal(identity);
                state = new AuthenticationState(user);

            }

            NotifyAuthenticationStateChanged(Task.FromResult(state));
            return await Task.FromResult(state);
        }

        private IEnumerable<Claim> ParseJWTClaims(string jwt)
        {
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WihoutPadding(payload);
            var kvPairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
            return (kvPairs ?? new Dictionary<string, object>()).Select(kv => new Claim(kv.Key, kv.Value.ToString()));
        }

        private byte[] ParseBase64WihoutPadding(string base64)
        {
            var x = base64.Length % 4;
            return Convert.FromBase64String((base64.Length % 4) switch
           {
               2 => base64 += "==",
               3 => base64 += "="
           });
        }
    } 
}
