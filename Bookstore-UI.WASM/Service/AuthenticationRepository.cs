using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Bookstore_UI.WASM.Contracts;
using Bookstore_UI.WASM.Models;
using Bookstore_UI.WASM.Providers;
using Bookstore_UI.WASM.Static;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.VisualBasic;
using Newtonsoft.Json;

namespace Bookstore_UI.WASM.Service
{
    public class AuthenticationRepository : IAuthenticationRepository
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
 
        public AuthenticationRepository(HttpClient httpClient,
            ILocalStorageService localStorage,
            AuthenticationStateProvider authenticationStateProvider)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<bool> Login(LoginModel user)
        {
            var response = await _httpClient.PostAsJsonAsync(Endpoints.LoginEndpoint, user);
            
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

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.Token);

            return true;
        }

        public async Task Logout()
        {
            await _localStorage.RemoveItemAsync("authToken");

            ((APIAuthenticationStateProvider)_authenticationStateProvider).DoLogout();
        }

        public async Task<bool> Register(RegistrationModel user)
        {
            var response = await _httpClient.PostAsJsonAsync(Endpoints.RegisterEndpoint, user);

            return response.IsSuccessStatusCode;
        }
    }
}
