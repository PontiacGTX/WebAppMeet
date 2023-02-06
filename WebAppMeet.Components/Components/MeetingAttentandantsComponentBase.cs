using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAppMeet.Components.Components
{
    public class MeetingAttentandantsComponentBase:ComponentBase
    {
        public Dictionary<string,string> Attendants { get; set; }

        public void AddAttendant(string id,string str)
        {
            if (!Attendants.ContainsKey(id))
            {
                Attendants.Add(id, str);
                StateHasChanged();
            }
        }
        public bool ContainsAttendant(string id)
        {
            return Attendants.ContainsKey(id);
        }
       
        public void RemoveAttendant(string str)
        { 
            Attendants.Remove(str);
        }
        public async Task ComponentStateHasChangedAsync()
        {
            await InvokeAsync(() => {
                StateHasChanged();
            });
        }
        protected override Task OnInitializedAsync()
        {
            if (Attendants == null) Attendants = new();

            return base.OnInitializedAsync();
        }
    }
}
