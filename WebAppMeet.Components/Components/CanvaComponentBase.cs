using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAppMeet.Components.Components
{
    public  class CanvaComponentBase:ComponentBase
    {
        protected IBrowserFile valFile;


        [Inject]
        protected IJSRuntime JS { get; set; }

        private IJSObjectReference _module;
        protected string CanvasId { get; set; } = "imageSelect";
        IFormFile file { get; set; }


        protected async Task LoadFile()
        {
          
            using var memoryStream = new MemoryStream();
            //await e.File.OpenReadStream(e.File.Size).CopyToAsync(memoryStream);
            var base64 = "";
            //await LoadImage(base64, CanvasId);
        }
        protected async Task OnFileSelected(InputFileChangeEventArgs e)
        {
            using var memoryStream = new MemoryStream();
            await e.File.OpenReadStream(e.File.Size).CopyToAsync(memoryStream);
            string base64 = $"data:{e.File.ContentType};base64,{Convert.ToBase64String(memoryStream.ToArray())}";
            await LoadImage(base64, CanvasId);
            this.StateHasChanged();
            //await using FileStream fs = new(path, FileMode.Create);
            //await browserFile.OpenReadStream().CopyToAsync(fs);
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
        protected async Task LoadImage(string base64,string canvasId)
        {
            try
            {
               var task = _module.InvokeVoidAsync("exportToCanvas", base64, canvasId);
               await task;

            }
            catch (Exception ex)
            {

                throw;
            }
        }
        protected async Task PrintMessage(string title, string message)
        {

            try
            {
                var item = _module.InvokeVoidAsync("showAlert", title, message);

            }
            catch (Exception ex)
            {

                throw;
            }
        }

    }
}
