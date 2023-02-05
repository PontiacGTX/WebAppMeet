using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAppMeet.Components.Components
{
    public class UserActivityIndicatorComponentBase:ComponentBase
    {
        public string messageActivity { get; set; }

        public async Task ComponentStateHasChangedAsync()
        {
            await InvokeAsync(() => {
                StateHasChanged();
            });
           
        }
    }
}
