using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAppMeet.Data.Entities
{
     public class UserMeetings: IEntity
    {
        [Key]
        public int UserMeetingsId { get; set; }
        public string HubIdCon { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public AppUser User { get; set; }
        public int MeetingId { get; set; }
        [ForeignKey("MeetingId")]
        public Meeting Meeting { get; set; }

        public bool AllowGuestAccess { get; set; }
        public bool IsActive { get; set; }
        public bool HasOfferedConnection { get; set; }
        public bool HasAcceptedConnection { get; set; }
        public bool IsHost { get; set; }

    }
}
