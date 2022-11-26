using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAppMeet.Data
{
    public class ErrorServerResponse<TR> : Response<TR>
    {
        public override int StatusCode { get; set; } = 500;
        public override bool Success { get; set; } = false;
        
    }
}