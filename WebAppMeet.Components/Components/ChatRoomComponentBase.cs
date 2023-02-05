using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WebAppMeet.Data.Entities;
using WebAppMeet.Data.Models;
using WebAppMeet.Services.Services;
using WebAppMeet.Data;
using Microsoft.AspNetCore.Identity;
using WebAppMeet.Components.Helper;
using Microsoft.AspNetCore.Components.Authorization;

namespace WebAppMeet.Components.Components
{

    public  class ChatRoomComponentBase:ComponentBase
    {

        protected AuthenticationState _state { get; set; }
        protected IJSObjectReference _module;
        [Parameter]
        public EventCallback<string> OnChatboxUserTyping { get; set; }
        [Parameter]
        public int MeetingId { get; set; }
        [Inject]
        protected AuthenticationStateProvider _AuthenticationStateProv { get; set; }
        public Action<string, string> HubOnReceiveMessageDelegate;
        [Inject]
        protected LocalStorage _localDataStorage { get; set; }
        [Inject]
        public UserManager<AppUser> _userManager { get; set; }
        [Inject]
        protected IJSRuntime JS { get; set; }
        [Inject]
        public MeetingsServices _meetingServices { get; set; }
        public Microsoft.AspNetCore.SignalR.Client.HubConnection hub { get; set; }

        protected Response<IList<UserMeetings>> _UserMeetingResponse { get; set; }
        [CascadingParameter]
        public AppUser User { get; set; }
        protected IList<UserMeetings> _UserMeetings { get; set; }
        public ChatBoxComponentBase ChatBox { get; set; }

        
        public async void OnReceiveMessage(string sender, string message)
        {
            var encodedMsg = $"{sender} {DateTime.Now.ToString()}: {message}";
            ChatBox.MessageList.Add(encodedMsg);
            await ChatBox.ComponentStateHasChanged();

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
        protected async Task<string> getTokenJs()
        {
            return await _module.InvokeAsync<string>("getLocalStorageToken");
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
        protected async Task OnUserTyping(string message)
        {
           await OnChatboxUserTyping.InvokeAsync(message);
        }
        protected async Task SendMessage(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                await PrintMessage("Error", "Cannot send an empty message");
                return;
            }
            if (hub is not null)
            {

                foreach (var user in _UserMeetings)
                {
                    await hub.SendAsync("SendMessage", User.Email, user.User.Email, ChatBox.Message);
                }

                 //await  ChatBox.ComponentStateHasChanged();
            }
        }

    }
}
