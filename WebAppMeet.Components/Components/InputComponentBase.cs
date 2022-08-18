using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAppMeet.Services.Services;

namespace WebAppMeet.Components.Components
{
    public class InputComponentBase:ComponentBase
    {
        protected IJSObjectReference _module;

        //public virtual string Val { get; set; }

        [Parameter]
        public virtual string Value { get; set; }
        [Parameter]
        public virtual string id { get; set; }

        [Inject]
        protected IJSRuntime JS { get; set; }

        

    }

}
