using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Blazored.LocalStorage;
using Blazored.Toast;
using Bookstore_UI.WASM.Data;
using Bookstore_UI.WASM.Providers;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using Bookstore_UI.WASM.Contracts;
using Bookstore_UI.WASM.Service;

namespace Bookstore_UI.WASM
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            builder.Services.AddBlazoredLocalStorage();
            builder.Services.AddBlazoredToast();
            builder.Services.AddSingleton<WeatherForecastService>();
            builder.Services.AddAuthorizationCore();
            builder.Services.AddScoped<APIAuthenticationStateProvider>();
            builder.Services.AddScoped<AuthenticationStateProvider>(p =>
                p.GetRequiredService<APIAuthenticationStateProvider>());
            builder.Services.AddScoped<JwtSecurityTokenHandler>();
            builder.Services.AddTransient<IAuthenticationRepository, AuthenticationRepository>();
            builder.Services.AddTransient<IAuthorRepository, AuthorRepository>();
            builder.Services.AddTransient<IBookRepository, BookRepository>();
            //builder.Services.AddTransient<IFileUpload, FileUpload>();

            await builder.Build().RunAsync();
        }
    }
}
