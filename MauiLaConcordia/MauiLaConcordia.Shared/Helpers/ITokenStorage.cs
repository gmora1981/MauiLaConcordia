using System.Threading.Tasks;

namespace MauiLaConcordia.Shared.Helpers
{
    public interface ITokenStorage
    {
        Task<string> GetToken(string key);
        Task SetToken(string key, string value);
        Task RemoveToken(string key);
    }
}