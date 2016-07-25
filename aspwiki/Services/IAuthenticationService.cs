using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ASPWiki.Model;

namespace ASPWiki.Services
{
    public interface IAuthenticationService
    {
        Task<bool> ValidateToken(string token, string id);
        Task<bool> CreateDevSession();
    }
}
