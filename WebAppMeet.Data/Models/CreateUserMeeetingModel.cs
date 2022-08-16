using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAppMeet.Data.Models
{
    public class CreateUserMeeetingModel
    {
        public string HubId { get; set; }
        public string HostId { get; set; }
        public string UserId { get; set; }
        public int MeetingId { get; set; }
        public DateTime Date { get; set; }
        public bool IsHost { get; set; }

    }
}
