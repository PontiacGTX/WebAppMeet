using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAppMeet.Components.Helper
{
    public class LocalStorage
    {
        ProtectedLocalStorage _ProtectedLocalStore { get; set; }
        public LocalStorage(ProtectedLocalStorage ProtectedLocalStore)
        {
            _ProtectedLocalStore= ProtectedLocalStore;
        }
        public async Task<string> GetTokenAsync()
        => (await _ProtectedLocalStore.GetAsync<string>("accessToken")).Value;

        public async Task SetTokenAsync(string token=null)
        {
           
            var task = token switch
            {
               null or "" => _ProtectedLocalStore.DeleteAsync("accessToken"),
                _ => _ProtectedLocalStore.SetAsync("accessToken", token),
            };
            await task;
        }

        public async Task<bool> ExistKey<T>(string key)
          => (await _ProtectedLocalStore.GetAsync<T>(key)).Success;

        public async Task<bool> Delete<T>(string key)
            where T : class
        {
            await _ProtectedLocalStore.DeleteAsync(key);

            return !(await _ProtectedLocalStore.GetAsync<T>(key)).Success;
        }

        public async Task<T> Get<T>(string key)
          => (await _ProtectedLocalStore.GetAsync<T>(key)).Value;

        public async Task SetAsync(string key, object value)
            => await _ProtectedLocalStore.SetAsync(key, value);
        
    }

}

