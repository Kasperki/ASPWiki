using ASPWiki.Model;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace ASPWiki.Services
{
    public class AuthenticationRequirement : IAuthorizationRequirement
    {
    }

    public class AuthenticationRequirementHandler : AuthorizationHandler<AuthenticationRequirement, WikiPage>
    {
        private readonly IAuthenticationService authenticationService;

        public AuthenticationRequirementHandler(IAuthenticationService authenticationService)
        {
            this.authenticationService = authenticationService;
        }

        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            AuthenticationRequirement requirement,
            WikiPage resource)
        {

            if (authenticationService.IsAuthenticatedAndWhiteListed())
            {
                context.Succeed(requirement);
            }

            return Task.FromResult(0);
        }
    }
}
