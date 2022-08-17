using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAppMeet.Data;
using WebAppMeet.Data.Entities;
using WebAppMeet.Services.Services;

namespace WebAppMeet.Components.Components
{
    public class MeetingListComponentBase:ComponentBase
    {
        [CascadingParameter]
        protected Task<AuthenticationState> _authenticationState { get; set; }

        [Parameter]
        public string UserId { get; set; }

        [Inject]
        public UserManager<AppUser> _UserManager { get; set; }
        [Inject]
        public NavigationManager _NavigationManager { get; set; }
        [Inject]
        public MeetingsServices _meetingServices { get; set; }
        [Inject]
        public UserServices _UserServices { get; set; }

        [Inject]
        IJSRuntime JS { get; set; }

        protected IJSObjectReference _module;
        protected IList<Meeting> meetings { get; set; }
        protected Response MeetingsResponse { get; set; }

        
        protected override async Task OnInitializedAsync()
        {
            var state = await _authenticationState;

            if (UserId == "0")
            {
                _NavigationManager.NavigateTo("/identity/account/login", true);
                return;
            }

            if (MeetingsResponse is null)
            {
                MeetingsResponse = await _meetingServices.GeAllGroupByMeeting(x => x.UserId == UserId);
                meetings = (MeetingsResponse?.Data as IList<Meeting>)??new List<Meeting>();
                this.StateHasChanged();
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
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                try
                {
                    _module = await JS.InvokeAsync<IJSObjectReference>(
                    "import", "./js/InterOpLib.js");
                }
                catch (Exception ex)
                {
                    throw;
                }

               
            }
            else
            {

                try
                {
                    await (MeetingsResponse.StatusCode switch
                    {
                        400 => PrintMessage("Error while requesting user", MeetingsResponse.Message),
                        404 => PrintMessage("Not Meetings were found", MeetingsResponse.Message),
                        500 => PrintMessage("A ServerError happened try again", MeetingsResponse.Message),
                        200=> Task.FromResult("")
                    });
                    meetings = (MeetingsResponse?.Data as IList<Meeting>) ?? new List<Meeting>();
                }
                catch (Exception ex)
                {
                    await PrintMessage("Unexpected error", ex.Message);
                }
            }
        }


    }
}
