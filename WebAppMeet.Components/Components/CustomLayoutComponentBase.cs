using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAppMeet.Components.Helper;
using WebAppMeet.Data;
using WebAppMeet.Data.Models;

namespace WebAppMeet.Components.Components
{
    public class CustomLayoutComponentBase:LayoutComponentBase
    {
        
        [Inject]
        public CustomAuthenticationStateProvider _AuthenticationStateProv { get; set; }

        [Inject]
        protected LocalStorage _localDataStorage { get; set; }

        [Inject]
        public UserManager<AppUser> _userManager { get; set; }
        protected AppUser User { get; set; }

        
        [Inject]
        protected IJSRuntime JS { get; set; }

        protected IJSObjectReference _module;

        protected async Task<string> getTokenJs()
        {
            return await _module.InvokeAsync<string>("getLocalStorageToken");
        }
        protected async Task<TokenResponse> getTokenDataJs()
        {
            TokenResponse dat = null;
            var str= await _module.InvokeAsync<string>("getLocalStorageTokenData");
            
            if(!string.IsNullOrEmpty(str))
            dat = JsonConvert.DeserializeObject<TokenResponse>(str);

            return dat;
        }
        protected async Task TrySaveJsToken(string token)
        {
            try
            {
                string interopToken = null;
                try
                {
                    interopToken = await _localDataStorage.GetTokenAsync();
                }
                catch (Exception)
                {

                }

                if (interopToken != token && token is not null ) 
                await _localDataStorage.SetTokenAsync(token);
            }
            catch (Exception)
            {
            }
        }
        protected async Task<TokenResponse> TryGetTokenData()
        {
            try
            {
                return await _localDataStorage.GetTokenData();
            }
            catch (Exception)
            {
                try
                {
                    return await getTokenDataJs();

                }
                catch (JSDisconnectedException ex)
                {
                    if(ex!=null)
                        StateHasChanged();
                }
                return null;
            }
        }

        protected async Task<string> TryGetToken()
        {
            try
            {
                var token =  await _localDataStorage.GetTokenAsync();
                if (token == null)
                    throw new Exception();
                return token;
            }
            catch 
            {
                try
                {

                    return await getTokenJs();
                }
                catch (Exception)
                {
                }
                return null;
            }
        }
    }
}
