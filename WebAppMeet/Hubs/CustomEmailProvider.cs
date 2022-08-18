using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace WebAppMeet.Hubs
{
    public class CustomEmailProvider : IUserIdProvider
    {
        public virtual string GetUserId(HubConnectionContext connection)
        {
            
            var res = connection.User?.Claims.FirstOrDefault(x=>x.Type==ClaimTypes.NameIdentifier && x.Value == connection?.User?.Identity?.Name);
            return res?.Value;
        }
    }
}
