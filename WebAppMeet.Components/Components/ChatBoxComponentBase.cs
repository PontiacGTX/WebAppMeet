using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAppMeet.Components.Components
{
    public class ChatBoxComponentBase:ComponentBase
    {
       
        public List<string> MessageList { get; set; }
        public string Message { get; set; }
        public string HubId { get; set; }
        protected override async Task OnInitializedAsync()
        {
            if (MessageList == null)
                MessageList = new List<string>();
        }

        protected async Task OnButtonSend(MouseEventArgs e)
        {

        }

        public async Task ComponentStateHasChanged()
        {
            StateHasChanged();
        }
    }
}
