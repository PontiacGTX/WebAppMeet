
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAppMeet.Data;
using WebAppMeet.Data.Entities;
using WebAppMeet.Data.Models;
using WebAppMeet.DataAcess.Factory;

namespace WebAppMeet.Services.Services
{
    public class ChatLogsServices: ServiceBaseProvider
    {
        public ChatLogsServices(GenericFactory factory)
        {
            _factory = factory;
        }

        public  async Task<Response<ChatLog>> Create<EntityModel>(EntityModel Model)
        {
           var model = Model as ChatLog;

           var chatLogRepo =await  GetRepository<ChatLog>();

           var item = new ChatLog {  IdUser = model.IdUser ,MeetingId = model.MeetingId };

           item =await chatLogRepo.AddAndSave(item);

            if (item is null)
                return Factory.GetResponse<ErrorServerResponse<ChatLog>, ChatLog>(null, messages: (new string[] { Factory.GetStringResponse(StringResponseEnum.InternalServerError, "ChatLog") }).ToArray());

            return Factory.GetResponse<Response<ChatLog>, ChatLog>(item);
        }

        public  async Task<Response<bool?>> Delete<TId>(TId id)
        {
            var chatLogRepo = await GetRepository<ChatLog>();

            var item = await chatLogRepo.Get(id);

            if(item is null)
            return Factory.GetResponse<Response<bool?>,bool?>(null, messages: new string[] { Factory.GetStringResponse(StringResponseEnum.NotFound, "chatlogId") });


            bool deleted =await chatLogRepo.Delete(item);

            return Factory.GetResponse<Response<bool?>,bool?>(deleted);
        }




    }
}
