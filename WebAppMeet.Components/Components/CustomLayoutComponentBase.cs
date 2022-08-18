using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAppMeet.Data;

namespace WebAppMeet.Components.Components
{
    public class CustomLayoutComponentBase:LayoutComponentBase
    {
        [Inject]
        public AuthenticationStateProvider _AuthenticationStateProv { get; set; }
        [Inject]
        public UserManager<AppUser> _userManager { get; set; }
        protected AppUser User { get; set; }
    }
}
