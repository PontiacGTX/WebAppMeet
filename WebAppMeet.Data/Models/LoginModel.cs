using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAppMeet.Data.Models
{
    public class LoginModel: CreateUserModel
    {
        public bool StayLoggedIn { get; set; }
    }
}
