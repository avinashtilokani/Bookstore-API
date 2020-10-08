using Blazored.LocalStorage;
using Bookstore_UI.WASM.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Bookstore_UI.WASM.Service
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;

        public BaseRepository(HttpClient httpClient,
            ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
        }
        public async Task<bool> Create(string url, T obj)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("bearer", await GetBearerToken());

            HttpResponseMessage response =  await _httpClient.PostAsJsonAsync<T>(url, obj);

            if (response.StatusCode == System.Net.HttpStatusCode.Created)
                return true;

            return false;
        }

        public async Task<bool> Delete(string url, int id)
        {
            if (id < 1)
                return false;
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("bearer", await GetBearerToken());

            HttpResponseMessage response = await _httpClient.DeleteAsync(url + id);
            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                return true;

            return false;
        }
        
        public async Task<T> Get(string url, int id)
        {

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("bearer", await GetBearerToken());

            var response = await _httpClient.GetFromJsonAsync<T>(url + id);
            
            return response;
        }

        public async Task<IList<T>> Get(string url)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("bearer", await GetBearerToken());

            var response = await _httpClient.GetFromJsonAsync<IList<T>>(url);

            return response;
        }

        public async Task<bool> Update(string url, T obj, int id)
        {
            if (obj == null)
                return false;

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("bearer", await GetBearerToken());

            var response = await _httpClient.PutAsJsonAsync<T>(url + id, obj);
           
            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                return true;

            return false;
        }

        private async Task<string> GetBearerToken()
        {
            return await _localStorage.GetItemAsync<string>("authToken");
        }
    }
}
