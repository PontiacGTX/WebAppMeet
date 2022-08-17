using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAppMeet.Data;
using WebAppMeet.Services.Services;

namespace WebAppMeet.Components
{
    public class MeetingComponentBase:ComponentBase
    {
        [Parameter]
        public int MeetingId { get; set; }
        protected AuthenticationState _state { get; set; }
        protected string UserId { get; set; }

        [Inject]
        public MeetingsServices _meetingServices { get; set; }
        [Inject]
        public UserManager<AppUser> _userManager { get; set; }
    }
}
