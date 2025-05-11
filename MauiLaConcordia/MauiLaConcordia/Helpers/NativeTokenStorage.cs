#if ANDROID || IOS || MACCATALYST || WINDOWS
using System.Threading.Tasks;

namespace MauiLaConcordia.Shared.Helpers
{
    public class NativeTokenStorage : ITokenStorage
    {
        public async Task<string> GetToken(string key)
        {
            return await SecureStorage.GetAsync(key);
        }

        public async Task SetToken(string key, string value)
        {
            await SecureStorage.SetAsync(key, value);
        }

        public async Task RemoveToken(string key)
        {
            SecureStorage.Remove(key);
        }
    }
}
#endif