using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAppMeet.Data
{
    public class Response
    {
        public virtual int StatusCode { get; set; } = 200;
        public string Message { get; set; } = "0k";
        public object Data { get; set; }
        public virtual bool Success { get; set; } = true;
        public IEnumerable<string> Validation { get; set; } = null;
    }
}
