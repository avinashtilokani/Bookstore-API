using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Bookstore_UI.Contracts;
using Bookstore_UI.Models;
using Bookstore_UI.Providers;
using Bookstore_UI.Static;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.VisualBasic;
using Newtonsoft.Json;

namespace Bookstore_UI.Service
{
    public class AuthenticationRepository : IAuthenticationRepository
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILocalStorageService _localStorage;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
 
        public AuthenticationRepository(IHttpClientFactory httpClientFactory,
            ILocalStorageService localStorage,
            AuthenticationStateProvider authenticationStateProvider)
        {
            _httpClientFactory = httpClientFactory;
            _localStorage = localStorage;
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<bool> Login(LoginModel user)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, Endpoints.LoginEndpoint);

            request.Content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8,
                "application/json");

            var client = _httpClientFactory.CreateClient();

            HttpResponseMessage response = await client.SendAsync(request);

            if(!response.IsSuccessStatusCode)
            {
                return false;
            }

            var content = await response.Content.ReadAsStringAsync();
            var token = JsonConvert.DeserializeObject<TokenResponse>(content);

            //store token
            await _localStorage.SetItemAsync("authToken", token.Token);

            //Change the authentication state of application
            await ((APIAuthenticationStateProvider)_authenticationStateProvider).DoLogIn();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.Token);

            return true;
        }

        public async Task Logout()
        {
            await _localStorage.RemoveItemAsync("authToken");

            ((APIAuthenticationStateProvider)_authenticationStateProvider).DoLogout();
        }

        public async Task<bool> Register(RegistrationModel user)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, Endpoints.RegisterEndpoint);

            request.Content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8,
                "application/json");

            var client = _httpClientFactory.CreateClient();

            HttpResponseMessage response = await client.SendAsync(request);

            return response.IsSuccessStatusCode;

        }
    }
}
