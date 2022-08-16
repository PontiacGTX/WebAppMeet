using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAppMeet.Components.Components
{
    public class OptionComponentBase:ComponentBase
    {
        [Parameter]
        public virtual string Value { get; set; }
        [Parameter]
        public virtual bool Selected { get; set; }
    }
}
