using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAppMeet.Data.Models
{
    public enum StringResponseEnum
    {
        FailedRequest = -1,
        NotFound = 0,
        InternalServerError = 1,
        BadRequestError = 2,
        RepositoryError = 3,
        Unathorized = 4,
        AlreadyInUse =5, 
        TimeOut=6,

    }
}
