using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAppMeet.Components.Components
{
    public class ChatBoxComponentBase : ComponentBase
    {
        protected ElementReference TextAreaRef;
        
        public UserActivityIndicatorComponentBase UserActivityIndicatorComponent { get; set; }
        [Inject]
        protected IJSRuntime JS { get; set; }

        private IJSObjectReference _module;
        public List<string> MessageList { get; set; }
        public string Message { get; set; }
        public string HubId { get; set; }
        [Parameter]
        public EventCallback<string> OnSendMessage { get; set; }
        [Parameter]
        public EventCallback<string> OnUserTyping { get; set; }
        protected override async Task OnInitializedAsync()
        {
            if (MessageList == null)
                MessageList = new List<string>();
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                try
                {
                    _module = await JS.InvokeAsync<IJSObjectReference>(
                    "import", "./js/InterOpLib.js");
                }
                catch (Exception)
                {
                    throw;
                }
            }

        }
        
        protected async Task ScrollToBottom()
        {
            
            await _module.InvokeVoidAsync("scrollToEnd", new object[] { TextAreaRef });
        }

        protected async Task OnButtonSend(MouseEventArgs e)
        {
            await SendAndClear();
            await OnUserTyping.InvokeAsync("Enter");
        }
        protected async Task OnKeyPressed(KeyboardEventArgs e)
        {
            if(!string.IsNullOrEmpty(Message) && e.Key!="Backspace")
            {
               await OnUserTyping.InvokeAsync(e.Key);
            }
            else if(string.IsNullOrEmpty(Message) && e.Key == "Backspace")
            {
               await OnUserTyping.InvokeAsync("Enter");

            }
            if (e.Key == "Enter")
            {
                await SendAndClear();
            }
        }
        protected async Task SendAndClear()
        {
            await OnSendMessage.InvokeAsync(Message);
            Message = "";
            await ScrollToBottom();
        }
        public async Task ComponentStateHasChanged()
        {
            StateHasChanged();
        }
    }
}
