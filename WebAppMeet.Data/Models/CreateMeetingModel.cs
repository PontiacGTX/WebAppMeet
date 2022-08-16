using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAppMeet.Data.Models
{
    public class CreateMeetingModel
    {
        public string UserId { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
    }
}
