
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAppMeet.Data;
using WebAppMeet.DataAcess.DataContext;

namespace WebAppMeet.DataAcess.UserStore
{
    public class AppUserStore : UserStore<AppUser>, IUserEmailStore<AppUser>
    {
        public AppUserStore(AppDbContext ctx):base(ctx)
        {

        }
    }
}