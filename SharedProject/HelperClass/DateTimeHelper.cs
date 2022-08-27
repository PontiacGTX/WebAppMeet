using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedProject.HelperClass
{
    public static class DateTimeHelper
    {
        public static DateTime ToDateTimeFormat(this DateTime date,DateTimeKind timeKind = DateTimeKind.Utc)
        {
            return DateTime.SpecifyKind(date, timeKind);
        }
    }

}
