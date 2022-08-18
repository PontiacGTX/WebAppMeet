using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
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
    public class MeetingComponentBase : ComponentBase
    {
        [Parameter]
        public int MeetingId { get; set; }
        protected AuthenticationState _state { get; set; }
        protected string UserId { get; set; }
        protected ChatBoxComponentBase ChatBox { get; set; }
        [Inject]
        protected AuthenticationStateProvider _AuthenticationStateProv { get; set; }
        [Inject]
        protected NavigationManager _NavigationManager { get; set; }

        [Inject]
        public MeetingsServices _meetingServices { get; set; }
        [Inject]
        public UserManager<AppUser> _userManager { get; set; }
        [Inject]
        IJSRuntime JS { get; set; }
        [CascadingParameter]
        public AppUser User { get; set; }
        protected IList<UserMeetings> _UserMeetings { get; set; }
        protected IJSObjectReference _module;

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
    }
}
