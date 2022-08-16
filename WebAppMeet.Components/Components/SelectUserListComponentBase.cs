using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAppMeet.Components.Components
{
    public class SelectUserListComponentBase : ComponentBase
    {
        [Parameter]
        public Dictionary<string, string> SelectedUsers { get; set; }= new ();

        [Parameter]
        public EventCallback<string> DeleteuserEvent { get; set; }
        protected override Task OnInitializedAsync()
        {
            if (SelectedUsers == null)
                SelectedUsers = new();

            return base.OnInitializedAsync();
        }

       
    }
}
