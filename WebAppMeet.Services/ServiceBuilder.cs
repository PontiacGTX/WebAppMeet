using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAppMeet.Data;
using WebAppMeet.Services.Services;

namespace WebAppMeet.Services
{
    public class ServiceBuilder
    {
         ServiceBaseProvider _Service { get; }
         UserManager<AppUser> _userManager { get; }
        public ServiceBuilder(ServiceBaseProvider serviceProvider,UserManager<AppUser> userManager)
        {
            _Service = serviceProvider;
            _userManager = userManager;
        }

        public async Task<IService> GetService<TService>()
            where TService:class, IService
        {
            TService Ts =null;
            return Ts switch
            {
                MeetingsServices t  => _Service ,
                UserServices t => new UserServices(await _Service.GetGenericRepository(), _userManager),
                _ => throw new Exception("Service doesnt exist")
            };
        }

        

    }
}
