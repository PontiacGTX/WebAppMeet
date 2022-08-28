using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAppMeet.Components.Components
{
    public class ChatBoxComponentBase : ComponentBase
    {

        public List<string> MessageList { get; set; }
        public string Message { get; set; }
        public string HubId { get; set; }
        [Parameter]
        public EventCallback<string> OnSendMessage { get; set; }
        protected override async Task OnInitializedAsync()
        {
            if (MessageList == null)
                MessageList = new List<string>();
        }

        protected async Task OnButtonSend(MouseEventArgs e)
        {
            await SendAndClear();
        }
        protected async Task OnKeyPressed(KeyboardEventArgs e)
        {
            if (e.Key == "Enter")
            {
                await SendAndClear();
            }
        }
        protected async Task SendAndClear()
        {
            await OnSendMessage.InvokeAsync(Message);
            Message = "";
        }
        public async Task ComponentStateHasChanged()
        {
            StateHasChanged();
        }
    }
}
