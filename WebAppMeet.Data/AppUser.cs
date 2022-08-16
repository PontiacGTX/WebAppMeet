using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAppMeet.Data.Entities;

namespace WebAppMeet.Data
{
    public class AppUser : IdentityUser, IEntity
    {

        public bool IsEnabled { get; set; }
        public IList<Meeting> HostedMeetings { get; set; }
        public IList<UserMeetings> InvitedMeetings { get; set; }

        public AppUser Copy()
        {
            return (AppUser)this.MemberwiseClone();
        }
    }
}
