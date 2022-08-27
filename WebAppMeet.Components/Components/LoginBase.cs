using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAppMeet.Components.Helper;
using WebAppMeet.Data;
using WebAppMeet.Services.Services;

namespace WebAppMeet.Components.Components
{
    public class LoginBase : ComponentBase
    {

        [CascadingParameter]
        protected Task<AuthenticationState> _authenticationState { get; set; }

        [Inject]
        AuthenticationStateProvider auth { get; set; }

        [Inject]
        protected LocalStorage _localDataStorage { get; set; }

        [Inject]
        protected NavigationManager _NavigationManager { get; set; }

        [Inject]
        UserServices UserServices { get; set; }
        [Inject]
        public SignInManager<AppUser> _signInManager { get; set; }

        [Inject]
        protected AuthTokenServices _authService { get; set; }

        [Inject]
        IJSRuntime JS { get; set; }

        protected IJSObjectReference _module;
        public WebAppMeet.Data.Models.LoginModel LoginModel { get; set; }


        protected override Task OnInitializedAsync()
        {
            LoginModel = new Data.Models.LoginModel();

            return base.OnInitializedAsync();
        }

        protected async Task PrintMessage(string title, string message)
        {

            try
            {
                var item = _module.InvokeVoidAsync("showAlert", title, message);

            }
            catch (Exception ex)
            {

                throw;
            }
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                try
                {
                    _module = await JS.InvokeAsync<IJSObjectReference>(
                    "import", "./js/InterOpLib.js");
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }


       
    }
}
