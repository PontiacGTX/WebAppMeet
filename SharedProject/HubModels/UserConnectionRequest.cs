﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedProject.HubModels
{
    public class UserConnectionRequest
    {
        public string Email { get; set; }
        public string HubConnectionId { get; set; }
        public string UserId { get; set; }
    }
}
