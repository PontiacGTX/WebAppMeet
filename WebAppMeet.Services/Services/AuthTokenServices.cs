﻿using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAppMeet.Data;

namespace WebAppMeet.Services.Services
{
    public class AuthTokenServices
    {
        HttpClient _httpClient { get; }

        IHostingEnvironment _hostingEnvironment { get; }

        public string Url(string path) => $"{path}";
        public AuthTokenServices(HttpClient httpClient, IHostingEnvironment hostingEnvironment)
        {
            _httpClient = httpClient;
            _hostingEnvironment = hostingEnvironment;
        }
        public async Task<Response> GetJWTToken(WebAppMeet.Data.Models.UserTokenRequest request)
        {
            var url = Url($"{await GetBaseUrl()}/Security/Token/Create");
            var response = await _httpClient.PostAsync(url, GetContent(request));
            return GetResponse(await response.Content.ReadAsStringAsync());
        }
       async Task<string> GetBaseUrl()
       {

            if (_hostingEnvironment.IsEnvironment("Development"))
                return "https://localhost:7044";

            var url = Url("https://checkip.amazonaws.com/");
            var response = await _httpClient.GetAsync(url);
            var ip = await response.Content.ReadAsStringAsync();

           return $"https://{ip}";
       }
        protected Response GetResponse(string data)
            =>  JsonConvert.DeserializeObject<Response>(data);
        protected HttpContent GetContent(object content)
            =>new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");

    }
}
