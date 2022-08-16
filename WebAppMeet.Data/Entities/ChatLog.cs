using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAppMeet.Data.Entities
{
    public class ChatLog:IEntity
    {
        public long ChatLogId { get; set; }
        public string IdUser { get; set; }
        [ForeignKey("IdUser")]
        public AppUser User { get; set; }
        [ForeignKey("MeetingId")]
        public int MeetingId { get; set; }
        public Meeting Meeting { get; set; }
    }
}
