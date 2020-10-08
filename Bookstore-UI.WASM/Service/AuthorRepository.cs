using Blazored.LocalStorage;
using Bookstore_UI.WASM.Contracts;
using Bookstore_UI.WASM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Bookstore_UI.WASM.Service
{
    public class AuthorRepository : BaseRepository<Author>, IAuthorRepository
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;

        public AuthorRepository(HttpClient httpClient,
            ILocalStorageService localStorage) : base(httpClient, localStorage)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
        }
    }
}

