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
using WebAppMeet.Data.Entities;
using WebAppMeet.Services.Services;

namespace WebAppMeet.Components.Components;

public class MeetingComponentBase : ComponentBase
{

    [Parameter]
    public int MeetingId { get; set; }
    protected string UserId { get; set; }
    [Inject]
    protected CustomAuthenticationStateProvider _state { get; set; }
    protected MeetingAttentandantsComponentBase _MeetingAttentandantsComponentBase { get; set; }
    protected ChatRoomComponentBase ChatRoom { get; set; }
    protected AuthenticationState _AuthenticationState { get; set; }
    [Inject]
    protected NavigationManager _NavigationManager { get; set; }

    [Inject]
    public MeetingsServices _meetingServices { get; set; }
    [Inject]
    public UserManager<AppUser> _userManager { get; set; }
    [Inject]
    protected IJSRuntime JS { get; set; }
    [CascadingParameter]
    public AppUser User { get; set; }
    protected IList<UserMeetings> _UserMeetings { get; set; }
    protected IJSObjectReference _module;
    protected Response<IList<UserMeetings>> _UserMeetingResponse { get; set; }

    protected Action<string, string> HubOnReceiveMessageDelegate;
    [Inject]
    protected LocalStorage _localDataStorage { get; set; }
    protected Microsoft.AspNetCore.SignalR.Client.HubConnection hub { get; set; }
    protected async Task<string> getTokenJs()
    {
        return await _module.InvokeAsync<string>("getLocalStorageToken");
    }
    protected async Task<string> TryGetToken()
    {
        try
        {
            var token = await _localDataStorage.GetTokenAsync();
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
