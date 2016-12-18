using System.Threading.Tasks;
using ASPWiki.Model;

namespace ASPWiki.Services
{
    public interface IAuthenticationService
    {
        Task<Session> ValidateToken(string token, string id);
        Task<Session> CreateDevSession();
    }
}
