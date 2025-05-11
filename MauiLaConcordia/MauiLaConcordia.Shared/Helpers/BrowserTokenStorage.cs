using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace MauiLaConcordia.Shared.Helpers
{
    public class BrowserTokenStorage : ITokenStorage
    {
        private readonly IJSRuntime js;

        public BrowserTokenStorage(IJSRuntime js)
        {
            this.js = js;
        }

        public async Task<string> GetToken(string key)
        {
            try
            {
                return await js.InvokeAsync<string>("localStorage.getItem", key);
            }
            catch
            {
                // En caso de error (pre-rendering), devolver null
                return null;
            }
        }

        public async Task SetToken(string key, string value)
        {
            try
            {
                await js.InvokeVoidAsync("localStorage.setItem", key, value);
            }
            catch
            {
                // Ignorar errores durante pre-rendering
            }
        }

        public async Task RemoveToken(string key)
        {
            try
            {
                await js.InvokeVoidAsync("localStorage.removeItem", key);
            }
            catch
            {
                // Ignorar errores durante pre-rendering
            }
        }
    }
}