using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Twitter;

namespace ASPWiki.Services
{
    public interface IAuthenticationService
    {
        Task OnCreateTicket(TwitterCreatingTicketContext context);

        Task OnRemoteFailure(FailureContext context);

        AuthenticationProperties GetAuthenticationProperties();
        Task CreateDevClaim();

        bool IsAuthenticated();

        bool IsAuthenticatedAndWhiteListed();
    }
}
