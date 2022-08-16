using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAppMeet.Data;
using WebAppMeet.Data.Models;
using WebAppMeet.Services.Services;

namespace WebAppMeet.Components.Components
{
    public class RegisterBase:ComponentBase
    {

        [CascadingParameter]
        protected Task<AuthenticationState> _authenticationState { get; set; }

        [Inject]
        UserServices UserServices { get; set; }

        public CreateUserModel UserCreationModel { get; set; }

        [Inject]
        IJSRuntime JS { get; set; }
        protected  IJSObjectReference _module;
        protected override async Task OnInitializedAsync()
        {
           
            UserCreationModel = new CreateUserModel();

            
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
        protected async Task CreateUser()
        {
            
            var email = UserCreationModel.Email.ToUpperInvariant();
            try
            {
                if ((bool)(await UserServices.Exists(x => x.NormalizedUserName == email)).Data)
                {
                    await PrintMessage("Email already exists", "Please try a different email");
                    return;
                }
            }
            catch (Exception ex)
            {

            }

            Response userResult = null;
            try
            {
                userResult = await UserServices.Create<CreateUserModel>(UserCreationModel);
            }
            catch (Exception)
            {

            }

            if(userResult != null)
            await (userResult.StatusCode switch
            {
                400 => PrintMessage("Error while registering user", userResult.Message),
                500=> PrintMessage("A ServerError happened try again", userResult.Message),
                _=>  PrintMessage("User Registered", userResult.Message)
            });

        }
    }
}
