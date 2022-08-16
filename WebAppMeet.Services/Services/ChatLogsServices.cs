using SharedProject.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAppMeet.Data;
using WebAppMeet.Data.Entities;
using WebAppMeet.Data.Models;

namespace WebAppMeet.Services.Services
{
    public class ChatLogsServices: ServiceBaseProvider
    {
        public ChatLogsServices(GenericFactory factory)
        {
            _factory = factory;
        }

        public override async Task<Response> Create<EntityModel>(EntityModel Model)
        {
           var model = Model as ChatLog;

           var chatLogRepo =await  GetRepository<ChatLog>();

           var item = new ChatLog {  IdUser = model.IdUser ,MeetingId = model.MeetingId };

           item =await chatLogRepo.AddAndSave(item);

            if (item is null)
                return Factory.GetResponse<ErrorServerResponse>(null, messages: (new string[] { Factory.GetStringResponse(StringResponseEnum.InternalServerError, "ChatLog") }).ToArray());

            return Factory.GetResponse<Response>(item!=null);
        }

        public override async Task<Response> Delete<TId>(TId id)
        {
            var chatLogRepo = await GetRepository<ChatLog>();

            var item = await chatLogRepo.Get(id);

            if(item is null)
            return Factory.GetResponse<Response>(null, messages: new string[] { Factory.GetStringResponse(StringResponseEnum.NotFound, "chatlogId") });


            bool deleted =await chatLogRepo.Delete(item);

            return Factory.GetResponse<Response>(deleted);
        }




    }
}
