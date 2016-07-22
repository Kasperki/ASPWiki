using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ASPWiki.Services
{
    public interface IAuthenticationService
    {
        Task<bool> ValidateToken(HttpContext context, string token, string id);
    }
}
