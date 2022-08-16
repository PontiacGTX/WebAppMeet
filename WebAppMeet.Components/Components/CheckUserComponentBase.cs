using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAppMeet.Services.Services;

namespace WebAppMeet.Components.Components
{
    public class CheckUserComponentBase : ComponentBase
    {
        public string spinnerClass { get; set; } = "spinner-sta";
        public InputComponentBase inputComponent { get; set; }= new InputComponentBase { Value = "" };
        protected SelectUserListComponentBase selectedUserList { get; set; }= new SelectUserListComponentBase();

        Type optionsType = typeof(OptionComponentBase);

        protected List<DynamicComponent> dcs;

        private IJSObjectReference _module;

        protected List<Dictionary<string, object>> param { get; set; }
        public Dictionary<string,string> InvitedUsers { get; set; }
        [Inject]

        protected UserServices _UserServices { get; set; }

        
        [Inject]
        protected IJSRuntime JS { get; set; }

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
        protected async Task Add(MouseEventArgs e)
        {
            if (string.IsNullOrEmpty(inputComponent?.Value))
            {
                await PrintMessage("Adding Member","Cannot add an empty value");
                return;
            }
            var validVal = await IsValid(inputComponent?.Value);
            if (validVal == -1 || validVal==0)
            {
                if(validVal ==-1)
                {
                    await PrintMessage("Adding Member", "Cannot find this user");
                    return;
                }
                else if (validVal == 0)
                {
                    await PrintMessage("Adding Member", "An Error happened while trying to check if member exist please try adding again or contact support");
                    return;
                }
            }

            if (!InvitedUsers.ContainsKey(inputComponent.Value))
            {
                InvitedUsers.Add(inputComponent.Value,inputComponent.Value);
                StateHasChanged();
            }
            
        }

        public async Task DeleteUser(string u)
        {
            if (this.selectedUserList.SelectedUsers.ContainsKey(u))
            {
                this.selectedUserList.SelectedUsers.Remove(u);
                StateHasChanged();
            }
        }
        public void ComponentStateHasChanged()
        {
            StateHasChanged();
        }

        protected override Task OnInitializedAsync()
        {
            if(InvitedUsers is null)
            InvitedUsers =new Dictionary<string, string>();
 
            if(selectedUserList is null)
                selectedUserList = new SelectUserListComponentBase();

            if (param == null)
           param = new List<Dictionary<string, object>>()
            {
                new Dictionary<string, object>()
                    {
                        { "Value","No Results" },
                        { "Selected",false}

                    },
            };
            
            if (dcs ==null)
            {
                dcs = new();
                //dcs.Add(new DynamicComponent() {  Parameters = param[0], Type = optionsType });
            }
            
            return base.OnInitializedAsync();
        }
        protected async Task<int> IsValid(string user)
        {
            var items = await _UserServices.UsersLike($"{this.inputComponent.Value}");
            return items.StatusCode switch
            {
                200 => (items.Data as List<string>).Any(x=>x== this.inputComponent.Value) ? 1 : -1,
                404 => -1,
                _=>0
            };

        }
        protected async Task Search(KeyboardEventArgs e)
        {

            if(!string.IsNullOrEmpty(this.inputComponent.Value) &&
                this.inputComponent.Value is { Length:> 1 } && e.Key=="Backspace" ||
                 this.inputComponent.Value is { Length:1 }  && e.Key != "Backspace" ||
                 this.inputComponent.Value is { Length: > 1 } && e.Key != "Backspace"||
                 e is null)
            {
                spinnerClass = "lds-ring";
               var items =await _UserServices.UsersLike($"{this.inputComponent.Value}");
               if(items.StatusCode==200)
               {
                    param = new List<Dictionary<string, object>>();
                    var strs = items.Data as List<string>;
                    if ((strs).Any())
                    {
                        foreach (var str in strs)
                        {
                            var parameters = new Dictionary<string, object> {
                            { "Value",str  },
                            { "Selected",true }};
                            param.Add(parameters);

                            //dcs.Add(new DynamicComponent { Type = optionsType, Parameters = parameters });
                        }
                    }else
                    {
                        param = new List<Dictionary<string, object>>();
                        param.Add(new Dictionary<string, object> {
                            { "Value","No Results"   },
                            { "Selected",true }});
                       // dcs.Add(new DynamicComponent { Type = optionsType, Parameters = param[0] });
                    }

                if (param[0]["Value"]!="No Results" && param.Count>1)
                 await Task.Delay(600);
                else if(param[0]["Value"] != "No Results" && param.Count==1)
                await Task.Delay(500);
                else
                 await Task.Delay(100);

                    spinnerClass = "lds-ring-disa";
                StateHasChanged();
                }
            }
            else
            {
                param = new List<Dictionary<string, object>>();
                param.Add(new Dictionary<string, object> {
                            { "Value","No Results"   },
                            { "Selected",true }});
                StateHasChanged();
            }
            
        }

    }
}
