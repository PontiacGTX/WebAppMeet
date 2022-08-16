using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAppMeet.Data.Entities;
using WebAppMeet.Data;
using WebAppMeet.Data.Models;
using WebAppMeet.Services.Services;

namespace WebAppMeet.Components.Components
{
    public class MeetingCreationComponentBase:ComponentBase
    {

        [CascadingParameter]
        protected Task<AuthenticationState> _authenticationState { get; set; }
        protected CheckUserComponentBase CheckUserComponentBase { get; set; }
        protected CreateMeetingModel CreateModel { get; set; }

        [Inject]
        protected MeetingsServices _MeetingServices { get; set; }
        [Inject]
        protected UserServices _UserServices { get; set; }
        [Inject]
        IJSRuntime JS { get; set; }
        protected IJSObjectReference _module;
        [Inject]
        NavigationManager _NavigationManager { get; set; }

        [Parameter]
        public string idUser { get; set; }
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
        protected override Task OnInitializedAsync()
        {
            CheckUserComponentBase = new CheckUserComponentBase();
            CreateModel = new();
            CreateModel.Date = DateTime.Now;
            CreateModel.UserId = idUser;
            CreateModel.Description = "";

            return base.OnInitializedAsync();
        }
        protected async Task SetDate(ChangeEventArgs e)
        {
            var time = Convert.ToDateTime(e.Value);

            CreateModel.Date= new DateTime( time.Year, time.Month,time.Day,time.Hour,time.Minute, 0, 0, DateTimeKind.Utc);
            var item =CreateModel.Date.ToString();
        }

        public async Task CreateMeeting(EditContext edit)
        {
            var date =DateTime.Now;
            if (date > CreateModel.Date)
            {
                await PrintMessage("Error while submitting data", "Selected date time is not valid, please set a date in the future");
                return;
            }

            if(!CheckUserComponentBase.InvitedUsers.Any())
            {
                await PrintMessage("Error while submitting data", "Cannot create a meeting without any invitees");
                
                return;
            }
            var thisUserResult = await _UserServices.GetBy(CreateModel.UserId);
            var thisUser = thisUserResult.Data as AppUser;
            var invitedUsed = CheckUserComponentBase.InvitedUsers.Keys.FirstOrDefault(x => x.ToUpperInvariant() == thisUser?.Email.ToUpperInvariant());
            if (invitedUsed!=null)
            {
                CheckUserComponentBase.InvitedUsers.Remove(invitedUsed);
            }
            if (!CheckUserComponentBase.InvitedUsers.Any())
            {
                await PrintMessage("Error while submitting data", "Cannot create a with yourself as an invitee");
                CheckUserComponentBase.ComponentStateHasChanged();
                return;
            }
            var response = await _MeetingServices.Create(CreateModel);
            Func<Task<Task>> fn =  (response.StatusCode switch
            {
                200 => async () => {

                    var meeting = response.Data as Meeting;
                    if (meeting != null)
                    {
                        await _MeetingServices.CreateUserMeeting(new CreateUserMeeetingModel { HubId ="", Date = CreateModel.Date, HostId = CreateModel.UserId, IsHost = true, UserId = CreateModel.UserId, MeetingId = meeting.MeetingId,  });
                        List<string> invited = new();

                        try
                        {
                            foreach (var user in this.CheckUserComponentBase.InvitedUsers.Keys)
                            {
                               var responseUser=await _UserServices.GetUser(user);
                               AppUser usr = responseUser.Data as AppUser;
                               await _MeetingServices.CreateUserMeeting(new CreateUserMeeetingModel 
                               { 
                                   Date = CreateModel.Date,
                                   HostId = CreateModel.UserId, 
                                   IsHost = true, 
                                   UserId = usr.Id, 
                                   MeetingId = meeting.MeetingId 
                               });
                               invited.Add(usr.Email);
                            }
                        }
                        catch (Exception)
                        {

                        } 
                        var users = $"{Environment.NewLine}" + string.Join( Environment.NewLine, invited);
                        await PrintMessage("Following users were invited:", users);
                    }
                    
                    return Task.CompletedTask;
                } ,
                400 => async () => PrintMessage("Validation errors on some fields", response.Message),
                500 => async () => PrintMessage("A Server Error happened try again", response.Message),
                _ => async  () => { return Task.CompletedTask; }
            });
            await fn.Invoke();
            if(response.Success)
            {
                _NavigationManager.NavigateTo($"/ChatList/{thisUser.Id}", true);
            }
        }
    }
}
