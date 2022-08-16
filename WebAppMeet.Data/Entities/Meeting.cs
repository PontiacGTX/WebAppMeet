using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAppMeet.Data.Entities
{
    public class Meeting: IEntity
    {
        [Key]
        public int MeetingId { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public string HostId { get; set; }
        [ForeignKey("HostId")]
        public AppUser Host { get; set; }
        //public string HubIdCon { get; set; }
        public bool Started { get; set; }
        public bool Finished { get; set; }
        public string Url { get; set; }
        public IList<UserMeetings> MeetingMembers { get; set; }
        public bool IsEnabled { get; set; }
        public IList<ChatLog> ChatLogs { get; set; }
        public IList<UserMeetings> Invitees { get; set; }
    }
}
