using Blazored.LocalStorage;
using Bookstore_UI.Contracts;
using Bookstore_UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Bookstore_UI.Service
{
    public class BookRepository: BaseRepository<Book>, IBookRepository
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILocalStorageService _localStorage;

        public BookRepository(IHttpClientFactory httpClientFactory,
            ILocalStorageService localStorage) : base(httpClientFactory, localStorage)
        {
            _httpClientFactory = httpClientFactory;
            _localStorage = localStorage;
        }
    }
}
