using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WebAppMeet.Data;
using WebAppMeet.Data.Entities;
using WebAppMeet.Data.Models;
using WebAppMeet.DataAcess.Factory;

namespace WebAppMeet.Services.Services
{
    public class MeetingsServices: ServiceBaseProvider
    {

        public MeetingsServices(GenericFactory factory)
        {
            _factory = factory;
        }


        public  async Task<Response<Meeting>> Create<EntityModel>(EntityModel Model)
        {
            var model = Model as CreateMeetingModel;

            var repo = await GetRepository<Meeting>();
            
            var result =await repo.AddAndSave(new Meeting { Url = $"", Date = model.Date, Description = model.Description, Started = false, HostId = model.UserId, IsEnabled=true   });
            
            result.Url = $"/Meeting/{result.MeetingId}";
            
            result =await repo.UpdateAndSave(result, result.MeetingId);

            return  Factory.GetResponse<Response<Meeting>,Meeting>(result);
        }


        public  async Task<Response<UserMeetings>> CreateUserMeeting(CreateUserMeeetingModel model)
        {
            

            var repo = await GetRepository<Meeting>();

            IList<Meeting> meetings = null;
            
            
                meetings =(await repo.GetAll<Meeting>(include: inc =>
                                                            inc.Include(x => x.MeetingMembers)
                                                           .Include(x => x.Host),
                                                       whereClause: x => x.HostId == model.HostId && x.Date == model.Date,
                                                       selector: x => x));

     
            var any = meetings.Any(x => x != null);

            if (!any)
                return Factory.GetResponse<ErrorServerResponse<UserMeetings>, UserMeetings>(null, messages: new string[] { Factory.GetStringResponse(StringResponseEnum.NotFound, "meeting"), });

            var userMeetingsRepo = await GetRepository<UserMeetings>();

            var result = await userMeetingsRepo.AddAndSave(new UserMeetings { AllowGuestAccess = true, UserId = model.UserId, IsActive = false, IsHost = model.IsHost, MeetingId = model.MeetingId, HubIdCon = model.HubId });

            return Factory.GetResponse<Response<UserMeetings>, UserMeetings>(result);
        }

        public async Task<Response<Meeting>> GetBy<TId>(TId id)
        {
            int idV = Convert.ToInt32(id);

            var repo =await GetRepository<Meeting>();

            var meeting = await repo.FirstOrDefault(x => x.MeetingId == idV);

            if (meeting is null)
                return Factory.GetResponse<ErrorServerResponse<Meeting>,Meeting>(meeting, statusCode:404, messages: new string[] { Factory.GetStringResponse(StringResponseEnum.BadRequestError, "meeting") });
            

            return Factory.GetResponse<Response<Meeting>,Meeting>(meeting);
        }
        public async Task<Response<IList<Meeting>>> GetAllMeetings()
            => Factory.GetResponse<Response<IList<Meeting>>,IList<Meeting>> ( await (await _factory.GetRepositoryAsync<UserMeetings>()).GetAll<UserMeetings, Meeting, Meeting>(
                include: x => x.Include( x => x.User).Include(x => x.Meeting).ThenInclude(x => x.Host),
                whereClause: x=>x.Meeting.IsEnabled,
                selector: x => x,
                groupBy: x => x.GroupBy(y => y.Meeting),
                x => x.Key)); 
        public async Task<Response<IList<Meeting>>> GetAllMeetings(Expression<Func<UserMeetings,bool>> selector)
            => Factory.GetResponse<Response<IList<Meeting>>,IList<Meeting>>(await (await _factory.GetRepositoryAsync<UserMeetings>()).GetAll<UserMeetings, Meeting, Meeting>(
                include: x => x.Include(x => x.User).Include(x => x.Meeting).ThenInclude(x => x.Host),
                whereClause: x => x.Meeting.IsEnabled,
                selector: x => x,
                groupBy: x => x.GroupBy(y => y.Meeting),
                x => x.Key));

        public async Task<Response<IList<UserMeetings>>> GetUserMeeting<TId>(TId meetingId)
        {
            int id = Convert.ToInt32(meetingId);
            var repo = await GetRepository<UserMeetings>();
            var   userMeetings =await repo.GetAll(include: x => x.Include(x=>x.User), whereClause:x => x.MeetingId == id,selector: x => x );

            if (userMeetings is null)
                return Factory.GetResponse<Response<IList<UserMeetings>>, IList<UserMeetings>>(null,statusCode:404, messages: new string[] { Factory.GetStringResponse(StringResponseEnum.NotFound, "User Meetings"), });

            return Factory.GetResponse<Response<IList<UserMeetings>>,IList<UserMeetings>>(userMeetings);
        }

        public async Task<Response<Meeting>> GetMeetingBy(Func<Meeting,bool> selector)
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
                return Factory.GetResponse<ErrorServerResponse<Meeting>,Meeting>(null, messages: new string[] { Factory.GetStringResponse(StringResponseEnum.NotFound, "meeting"),  });

            return Factory.GetResponse<Response<Meeting>,Meeting>(meeting);

        }

        public async Task<Response<UserMeetings>> SetUserPresence(Expression<Func<UserMeetings,bool>> selector,Func<UserMeetings,UserMeetings> setter,bool isHost)
        {
            var userMeetingsrepo = await GetRepository<UserMeetings>();

            var userMeeting = await userMeetingsrepo.FirstOrDefault<UserMeetings>(include:inc=> {
                return inc.Include(x => x.User);
                },whereClause: selector, selector:x=>x);

            setter(userMeeting);

            var result =await userMeetingsrepo.UpdateAndSave(userMeeting, userMeeting.MeetingId);

            return Factory.GetResponse<Response<UserMeetings>,UserMeetings>(result);

        }

        async Task<Response<bool?>> RemoveGuestByEmail(string guestEmail, int meetingId)
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
                return Factory.GetResponse<ErrorServerResponse<bool?>,bool?>(null,statusCode:404, messages: new string[] { Factory.GetStringResponse(StringResponseEnum.BadRequestError, "guest") });


            var userMeetingsrepo = await GetRepository<UserMeetings>();

            bool deleted= await userMeetingsrepo.Delete(guestmeeting?.UserMeetingsId);

            return Factory.GetResponse<Response<bool?>,bool?>(deleted);
        }


        public async Task<Response<bool?>> RemoveGuestById(string userId,int meetingId)
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
                return Factory.GetResponse<ErrorServerResponse<bool?>,bool?>(null, statusCode: 404, messages: new string[] { Factory.GetStringResponse(StringResponseEnum.BadRequestError, "guest") });


            var userMeetingsrepo = await GetRepository<UserMeetings>();

            bool deleted = await userMeetingsrepo.Delete(guestmeeting?.UserMeetingsId);

            return Factory.GetResponse<Response<bool?>,bool?>(deleted);
        }


        public async Task<Response<bool?>> RemoveGuestBy(RemoveGuestByModel model)
        {

            return model.ParamType switch
            {
                IdentifierTypeEnum.Email=> await RemoveGuestByEmail(model.Identifier, model.MeetingId),
                IdentifierTypeEnum.Id=> await RemoveGuestById(model.Identifier,model.MeetingId),
                _ =>throw new Exception()
            };

        }

        

        public async Task<Response<Meeting>> GetBy(Expression<Func<Meeting, bool>> selector)
        {
            var repo = await GetRepository<Meeting>();


            if (!await repo.Any(selector))
                return Factory.GetResponse<Response<Meeting>,Meeting>(null,statusCode:404, messages: new string[] { Factory.GetStringResponse(StringResponseEnum.NotFound, "meetings ") });



            var meeting = await repo.FirstOrDefault<Meeting>(include: inc =>
            {
                return inc.Include(x => x.MeetingMembers)
                          .ThenInclude(x => x.User)
                          .Include(x => x.Host);
            },
            whereClause:selector,
            x=>x);



            return Factory.GetResponse<Response<Meeting>,Meeting>(meeting);
        }


        public async Task<Response<IList<Meeting>>> GeAllGroupByMeeting(Expression<Func<UserMeetings, bool>> filter)
        {
            var repo = await GetRepository<UserMeetings>();


            if (!await repo.Any(filter))
                return Factory.GetResponse<Response<IList<Meeting>>, IList<Meeting>>(null, statusCode: 404, messages: new string[] { Factory.GetStringResponse(StringResponseEnum.NotFound, "meetings ") });


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


            return Factory.GetResponse<Response<IList<Meeting>>, IList<Meeting>>(meetings);
        }
        async Task<Response<bool>> DeleteUserMeetingVyMeeting<TId>(TId meetingId)
        {
            var repo = await _factory.GetRepositoryAsync<UserMeetings>();

            IEnumerable<UserMeetings> userMeetings = await repo.GetAll<UserMeetings>(whereClause: (userMeeting => userMeeting.MeetingId == Convert.ToInt32(meetingId)), selector: x => x);

            await repo.DeleteRange(userMeetings);

            return  Factory.GetResponse<Response<bool>, bool>(true);
        }
        public  async Task<Response<bool?>> Delete<TId>(TId id)
        {
            await DeleteUserMeetingVyMeeting(id);

            var repo = await GetRepository<Meeting>();

            int idV = Convert.ToInt32(id);

            if (!await repo.Any(x => x.MeetingId == idV))
                return Factory.GetResponse<Response<bool?>, bool?>(null, messages: new string[] { Factory.GetStringResponse(StringResponseEnum.NotFound, "meeting") });

            var meeting = await repo.FirstOrDefault(x => x.MeetingId == idV);

            meeting.IsEnabled = false;

            await repo.SaveChanges();

            return Factory.GetResponse<Response<bool?>, bool?>(meeting.IsEnabled);
        }

      
    }
}
