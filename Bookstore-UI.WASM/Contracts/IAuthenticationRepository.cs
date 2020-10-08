using Bookstore_UI.WASM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bookstore_UI.WASM.Contracts
{
    public interface IAuthenticationRepository
    {

        public Task<bool> Register(RegistrationModel user);

        public Task<bool> Login(LoginModel user);

        public Task Logout();
    }
}
