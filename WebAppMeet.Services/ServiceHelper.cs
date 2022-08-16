using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAppMeet.Services.Services;

namespace WebAppMeet.Services
{
    public static class ServiceHelper
    {
        public static async Task<T> Cast<T>(this IService service)
        {
            return (T)service;
        }
    }
}
