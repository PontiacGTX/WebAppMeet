﻿using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace WebAppMeet.Hubs
{
    public class EmailBasedUserIdProvider : IUserIdProvider
    {
        public virtual string GetUserId(HubConnectionContext connection)
        {
            return connection.User?.FindFirst(ClaimTypes.Email)?.Value!;
        }
    }
}
