using Microsoft.AspNetCore.SignalR;
using WebAppMeet.Data.Entities;
using WebAppMeet.DataAcess.Repository;
using Microsoft.EntityFrameworkCore;
using SharedProject.HubModels;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using WebAppMeet.DataAcess.Factory;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace WebAppMeet.Hubs
{
    public interface IConnectionHub
    {
        
        Task CallAccepted(UserConnectionRequest acceptingUser);
        Task CallDeclined(UserConnectionRequest decliningUser, string reason);
        Task IncomingCall(UserMeetings callingUser);
        Task ReceiveSignal(UserConnectionRequest signalingUser, string signal);
        Task CallEnded(UserMeetings signalingUser, string signal);
        
    }
    //[Authorize(AuthenticationSchemes =JwtBearerDefaults.AuthenticationScheme)]
    public class ConnectionHub:Hub/*<IConnectionHub>*/
    {
        EntityRepository<Meeting> meetingsRepository { get; set; }
        EntityRepository<UserMeetings> userMeetingsRepository { get; set; }
        public ConnectionHub(GenericFactory genericFactory)
        {
            meetingsRepository = genericFactory.GetRepository<Meeting>();
            userMeetingsRepository = genericFactory.GetRepository<UserMeetings>();

        }

        //private Meeting GetMeeting(UserConnectionRequest request)
        //{
        //    var matchingCall =
        //       meetingsRepository.FirstOrDefault(x => { return x.Include(x => x.MeetingMembers); }, x => x.MeetingMembers.Any(x => x.HubIdCon == request.HubConnectionId || x.UserId == x.UserId), X => X).GetAwaiter().GetResult();

        //    return matchingCall;
        //}
        //private object GetUserCall(UserConnectionRequest request,bool onlyFirst)
        //{

        //    var matchingCall =
        //        meetingsRepository.FirstOrDefault(x => { return x.Include(x=>x.MeetingMembers); },x=>x.MeetingMembers.Any(x=>x.UserId == request.UserId && x.HubIdCon == request.HubConnectionId),X=>X.MeetingMembers ).GetAwaiter().GetResult();

        //    return onlyFirst? matchingCall.FirstOrDefault(x=>x!=null) : matchingCall;
        //}
        //private UserMeetings GetUser(UserConnectionRequest request)
        //{
        //    var matchingCall =
        //       meetingsRepository.FirstOrDefault(x => { return x.Include(x => x.MeetingMembers); }, x => x.MeetingMembers.Any(x => x.HubIdCon == request.HubConnectionId), X => X.MeetingMembers).GetAwaiter().GetResult();

        //    return matchingCall.FirstOrDefault(x => x != null);
        //}
        //private UserMeetings GetUserByEmail(UserConnectionRequest request)
        //{
        //    var matchingCall =
        //       meetingsRepository.FirstOrDefault(x => { return x.Include(x => x.MeetingMembers); }, x => x.MeetingMembers.Any(x => x.HubIdCon == request.HubConnectionId && x.User.Email == request.Email), X => X.MeetingMembers).GetAwaiter().GetResult();

        //    return matchingCall.FirstOrDefault(x => x != null);
        //}
        //public async Task CallUser(UserConnectionRequest targetConnectionId)
        //{
        //    var callingUser = GetUser(new UserConnectionRequest { HubConnectionId = Context.ConnectionId });
        //    var targetUser = GetUser(new UserConnectionRequest { HubConnectionId = targetConnectionId.HubConnectionId });

        //    // Make sure the person we are trying to call is still here
        //    if (targetUser == null)
        //    {
        //        // If not, let the caller know
        //        await Clients.Caller.CallDeclined(targetConnectionId, "The user you called has left.");
        //        return;
        //    }

        //    // And that they aren't already in a call
        //    if (GetUserCall(new UserConnectionRequest { HubConnectionId = targetConnectionId.HubConnectionId },true) != null)
        //    {
        //        await Clients.Caller.CallDeclined(targetConnectionId, string.Format("{0} is already in a call.", targetUser.User.Email));
        //        return;
        //    }

        //    // They are here, so tell them someone wants to talk
        //    await Clients.Client(targetConnectionId.HubConnectionId).IncomingCall(callingUser);

        //    // Create an offer

        //    callingUser.HasOfferedConnection = true;
        //    callingUser.IsActive = true;
        //    callingUser.HubIdCon = Context.ConnectionId;
        //    userMeetingsRepository.UpdateAndSave(callingUser, callingUser.UserId).GetAwaiter().GetResult();
        //}
        //public async Task Join(string username)
        //{

        //    var callingUser = GetUserByEmail(new UserConnectionRequest {  HubConnectionId = Context.ConnectionId, UserId = username });
        //    callingUser.HubIdCon = Context.ConnectionId;
        //    userMeetingsRepository.UpdateAndSave(callingUser, callingUser.UserId).GetAwaiter().GetResult();
        //}
        //public async Task AnswerCall(bool acceptCall, UserMeetings targetConnectionId)
        //{
        //    var callingUser = GetUser(new UserConnectionRequest { HubConnectionId = Context.ConnectionId });
        //    var targetUser = GetUser(new UserConnectionRequest { HubConnectionId = targetConnectionId.HubIdCon });

        //    // This can only happen if the server-side came down and clients were cleared, while the user
        //    // still held their browser session.
        //    if (callingUser == null)
        //    {
        //        return;
        //    }

        //    bool? isConnected = !(bool?)targetUser?.IsActive!;
        //    // Make sure the original caller has not left the page yet
        //    if ((bool)(bool?)(targetUser == null) || (bool)(isConnected ??false))
        //    {
        //        await Clients.Caller.CallEnded(targetConnectionId, "The other user in your call has left.");
        //        return;
        //    }

        //    // Send a decline message if the callee said no
        //    if (acceptCall == false)
        //    {
        //        await Clients.Client(targetConnectionId.HubIdCon).CallDeclined(new UserConnectionRequest { Email = targetConnectionId?.User?.Email, HubConnectionId = Context.ConnectionId, UserId = targetConnectionId.UserId}, string.Format("{0} did not accept your call.", callingUser?.User?.Email));
        //        return;
        //    }

        //    // Make sure there is still an active offer.  If there isn't, then the other use hung up before the Callee answered.
        //    var meeting = GetMeeting(new UserConnectionRequest { HubConnectionId = targetConnectionId.HubIdCon, UserId = targetConnectionId.UserId });

        //    if (meeting is null || meeting?.IsEnabled is false )
        //    {
        //        await Clients.Caller.CallEnded(targetConnectionId, string.Format("{0} has already hung up.", targetUser.User.Email));
        //        return;
        //    }

        //    // And finally... make sure the user hasn't accepted another call already
        //    if (GetUserCall(new UserConnectionRequest { HubConnectionId = targetUser.HubIdCon },true) != null)
        //    {
        //        // And that they aren't already in a call
        //        await Clients.Caller.CallDeclined(new UserConnectionRequest { HubConnectionId = targetConnectionId.HubIdCon, UserId = targetConnectionId.UserId }, string.Format("{0} chose to accept someone elses call instead of yours :(", targetUser.User.Email));
        //        return;
        //    }

        //    // Remove all the other offers for the call initiator, in case they have multiple calls out
        //    //_CallOffers.RemoveAll(c => c.Caller.ConnectionId == targetUser.ConnectionId);

        //    // Create a new call to match these folks up
        //    //_UserCalls.Add(new UserCall
        //    //{
        //    //    Users = new List<User> { callingUser, targetUser }
        //    //});

        //    // Tell the original caller that the call was accepted
        //    await Clients.Client(targetConnectionId.HubIdCon).CallAccepted(new UserConnectionRequest { HubConnectionId = targetConnectionId.HubIdCon, UserId = targetConnectionId.UserId });

        //    // Update the user list, since thes two are now in a call

        //}
        //public async Task HangUp()
        //{
        //    var callingUser = GetUser(new UserConnectionRequest { HubConnectionId = Context.ConnectionId });

        //    if (callingUser == null)
        //    {
        //        return;
        //    }

        //    var currentCall = GetUserCall(new UserConnectionRequest { HubConnectionId = Context.ConnectionId },false) as IList<UserMeetings>;

        //    // Send a hang up message to each user in the call, if there is one
        //    if (currentCall != null)
        //    {
        //        foreach (var user in currentCall.Where(u => u.HubIdCon != callingUser.HubIdCon))
        //        {
        //            await Clients.Client(user.HubIdCon).CallEnded(callingUser, string.Format("{0} has hung up.", callingUser.User.Email));
        //        }

        //        // Remove the call from the list if there is only one (or none) person left.  This should
        //        // always trigger now, but will be useful when we implement conferencing.
        //        currentCall.Where(u => u.HubIdCon == callingUser.HubIdCon);
        //        if (currentCall.Count >0)
        //        {
        //            await Clients.Clients(Context.ConnectionId).CallEnded(callingUser, string.Format("{0} has hung up.", callingUser.User.Email));
        //        }
        //    }


        //}
        public override async Task OnConnectedAsync()
        {
            var userName = Context.User?.Claims.FirstOrDefault(x => x.Type == "email")?.Value;

            if (!string.IsNullOrEmpty(userName))
                  await Groups.AddToGroupAsync(Context.ConnectionId, userName);

            await base.OnConnectedAsync();
        }

        public async Task JoinRoom(string id,string user)
        {
            await Clients.All.SendAsync("JoinedRoom",id, user);
        }
        public async Task SendMessage(string sender, string receiver, string message)
        {
            string name = Context.User.Claims.FirstOrDefault(x=>x.Type =="email")?.Value;
            var ctx = this.Clients;

         // if (sender != receiver)
             //  await Clients.Users(sender, receiver).SendAsync("ReceiveMessage", sender, message);
         // else
            {

               // await Clients.User(receiver).SendAsync("ReceiveMessage", message);
                //await Clients.Groups(name).SendAsync("ReceiveMessage", receiver, message);
                 //await this.Clients.Client(receiver).SendAsync("ReceiveMessage", sender, message);
                 if(sender!=receiver)
                    await Clients.Users(sender, receiver).SendAsync("ReceiveMessage", sender, message);
            } //this works await
               // Clients.All.SendAsync("ReceiveMessage", receiver, message);
        }


        //public async Task SendMessage(string username,string message)
        //{
        //   await Clients.User(username).SendAsync("ReceiveMessage", message);
        //}
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            
        }
    }
}
