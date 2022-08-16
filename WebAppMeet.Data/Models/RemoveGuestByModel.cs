using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAppMeet.Data.Models
{
    public class RemoveGuestByModel
    {
        public string Identifier { get; set; }
        public IdentifierTypeEnum ParamType { get; set; }
        public int MeetingId { get; set; }
    }
}
