using Microsoft.EntityFrameworkCore;
using SharedProject.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WebAppMeet.Data;
using WebAppMeet.Data.Entities;
using WebAppMeet.Data.Models;
using WebAppMeet.DataAcess.Repository;

namespace WebAppMeet.Services.Services
{
    public class MeetingsServices: ServiceBaseProvider
    {

        public MeetingsServices(GenericFactory factory)
        {
            _factory = factory;
        }


        public override async Task<Response> Create<EntityModel>(EntityModel Model)
        {
            var model = Model as CreateMeetingModel;

            var repo = await GetRepository<Meeting>();
            
            var result =await repo.AddAndSave(new Meeting { Url = $"", Date = model.Date, Description = model.Description, Started = false, HostId = model.UserId   });
            
            result.Url = $"/Meeting/{result.MeetingId}";
            
            result =await repo.UpdateAndSave(result, result.MeetingId);

            return  Factory.GetResponse<Response>(result);
        }


        public  async Task<Response> CreateUserMeeting(CreateUserMeeetingModel model)
        {
            

            var repo = await GetRepository<Meeting>();
            
            var meetings = (await repo.GetAll<Meeting>(include: inc =>
                                                            inc.Include(x => x.MeetingMembers)
                                                           .Include(x => x.Host),
                                                       whereClause: x => x.HostId == model.HostId && x.Date == model.Date,
                                                       selector: x => x));

            var any = meetings.Any(x => x != null);

            if (!any)
                return Factory.GetResponse<ErrorServerResponse>(null, messages: new string[] { Factory.GetStringResponse(StringResponseEnum.NotFound, "meeting"), });

            var userMeetingsRepo = await GetRepository<UserMeetings>();

            var result = await userMeetingsRepo.AddAndSave(new UserMeetings { AllowGuestAccess = true, UserId = model.UserId, IsActive = false, IsHost = model.IsHost, MeetingId = model.MeetingId, HubIdCon = model.HubId });

            return Factory.GetResponse<Response>(result);
        }

        public async Task<Response> GetBy<TId>(TId id)
        {
            int idV = Convert.ToInt32(id);

            var repo =await GetRepository<Meeting>();

            var meeting = repo.FirstOrDefault(x => x.MeetingId == idV);

            if (meeting is null)
                return Factory.GetResponse<ErrorServerResponse>(null, messages: new string[] { Factory.GetStringResponse(StringResponseEnum.BadRequestError, "meeting") });
            

            return Factory.GetResponse<Response>(meeting);
        }

        public async Task<Response> GetMeetingBy(Func<Meeting,bool> selector)
        {
            var meetingrepo = await GetRepository<Meeting>();

            var meetings = (await meetingrepo.GetAll<Meeting>(include: inc =>
                                                             inc.Include(x => x.MeetingMembers)
                                                            .ThenInclude(x => x.User)
                                                            .Include(x=>x.Host),
             x =>  selector(x),
             x => x));

            var meeting = meetings?.FirstOrDefault(x=>x!=null);

            if (meeting is null)
                return Factory.GetResponse<ErrorServerResponse>(null, messages: new string[] { Factory.GetStringResponse(StringResponseEnum.NotFound, "meeting"),  });

            return Factory.GetResponse<Response>(meeting);

        }

        public async Task<Response> SetUserPresence(Expression<Func<UserMeetings,bool>> selector,Func<UserMeetings,UserMeetings> setter,bool isHost)
        {
            var userMeetingsrepo = await GetRepository<UserMeetings>();

            var userMeeting = await userMeetingsrepo.FirstOrDefault<UserMeetings>(include:inc=> {
                return inc.Include(x => x.User);
                },whereClause: selector, selector:x=>x);

            setter(userMeeting);

            var result =await userMeetingsrepo.UpdateAndSave(userMeeting, userMeeting.MeetingId);

            return Factory.GetResponse<Response>(result);

        }

        async Task<Response> RemoveGuestByEmail(string guestEmail, int meetingId)
        {
            var meetingrepo = await GetRepository<Meeting>();

            var meetings = (await meetingrepo.GetAll(inc =>
                                                        inc.Include(x => x.MeetingMembers)
                                                           .ThenInclude(x => x.User),
            x=>x.MeetingMembers.Any(x=>x.User.Email == guestEmail) && x.MeetingId == meetingId,
            x=>x));

            var meeting = meetings.FirstOrDefault(x => x != null);

            var email = guestEmail.ToLower();

            var guestmeeting = meeting?.MeetingMembers.FirstOrDefault(x => x.User.Email.ToLower() == email);

            if(guestmeeting is null)
                return Factory.GetResponse<ErrorServerResponse>(null,statusCode:404, messages: new string[] { Factory.GetStringResponse(StringResponseEnum.BadRequestError, "guest") });


            var userMeetingsrepo = await GetRepository<UserMeetings>();

            bool deleted= await userMeetingsrepo.Delete(guestmeeting?.UserMeetingsId);

            return Factory.GetResponse<Response>(deleted);
        }


        public async Task<Response> RemoveGuestById(string userId,int meetingId)
        {
            var meetingrepo = await GetRepository<Meeting>();

            var meetings = (await meetingrepo.GetAll(inc =>
                                                        inc.Include(x => x.MeetingMembers)
                                                           .ThenInclude(x => x.User),
            x => x.MeetingMembers.Any(x => x.User.Id == userId && x.MeetingId == meetingId),
            x => x.MeetingMembers));

            var meeting = meetings.FirstOrDefault(x => x != null);



            var guestmeeting = meeting?.FirstOrDefault(x => x.UserId == userId);

            if (guestmeeting is null)
                return Factory.GetResponse<ErrorServerResponse>(null, statusCode: 404, messages: new string[] { Factory.GetStringResponse(StringResponseEnum.BadRequestError, "guest") });


            var userMeetingsrepo = await GetRepository<UserMeetings>();

            bool deleted = await userMeetingsrepo.Delete(guestmeeting?.UserMeetingsId);

            return Factory.GetResponse<Response>(deleted);
        }


        public async Task<Response> RemoveGuestBy(RemoveGuestByModel model)
        {

            return model.ParamType switch
            {
                IdentifierTypeEnum.Email=> await RemoveGuestByEmail(model.Identifier, model.MeetingId),
                IdentifierTypeEnum.Id=> await RemoveGuestById(model.Identifier,model.MeetingId),
                _ =>throw new Exception()
            };

        }



        public async Task<Response> GetBy(Expression<Func<Meeting, bool>> selector)
        {
            var repo = await GetRepository<Meeting>();


            if (!await repo.Any(selector))
                return Factory.GetResponse<Response>(null,statusCode:404, messages: new string[] { Factory.GetStringResponse(StringResponseEnum.NotFound, "meetings ") });



            var meeting = await repo.FirstOrDefault<Meeting>(include: inc =>
            {
                return inc.Include(x => x.MeetingMembers)
                          .ThenInclude(x => x.User)
                          .Include(x => x.Host);
            },
            whereClause:selector,
            x=>x);



            return Factory.GetResponse<Response>(meeting);
        }


        public async Task<Response> GeAllGroupByMeeting(Expression<Func<UserMeetings, bool>> filter)
        {
            var repo = await GetRepository<UserMeetings>();


            if (!await repo.Any(filter))
                return Factory.GetResponse<Response>(null, statusCode: 404, messages: new string[] { Factory.GetStringResponse(StringResponseEnum.NotFound, "meetings ") });


            IList<Meeting> meetings = null;
            try
            {
                meetings = await repo.GetAll<UserMeetings, Meeting, Meeting>(
                include: x => x.Include(x => x.User).Include(x => x.Meeting).ThenInclude(x => x.Host),
                whereClause: filter,
                selector: x => x,
                groupBy: x => x.GroupBy(y => y.Meeting),
                x => x.Key);
            }
            catch (Exception ex)
            {

                throw;
            }


            return Factory.GetResponse<Response>(meetings);
        }

        public override async Task<Response> Delete<TId>(TId id)
        {
            var repo = await GetRepository<Meeting>();

            int idV = Convert.ToInt32(id);

            if (!await repo.Any(x => x.MeetingId == idV))
                return Factory.GetResponse<Response>(null, messages: new string[] { Factory.GetStringResponse(StringResponseEnum.NotFound, "meeting") });

            var meeting = await repo.FirstOrDefault(x => x.MeetingId == idV);

            meeting.IsEnabled = false;

            await repo.SaveChanges();

            return Factory.GetResponse<Response>(meeting.IsEnabled);
        }

      
    }
}
