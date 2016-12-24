using ASPWiki.Model;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace ASPWiki.Services
{
    public class AnonymousRequirement : IAuthorizationRequirement
    {
    }

    public class AnonymousRequirementHandler : AuthorizationHandler<AnonymousRequirement, WikiPage>
    {
        private readonly IAuthenticationService authenticationService;

        public AnonymousRequirementHandler(IAuthenticationService authenticationService)
        {
            this.authenticationService = authenticationService;
        }

        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            AnonymousRequirement requirement,
            WikiPage resource)
        {
            if (resource.Public == true || authenticationService.IsAuthenticatedAndWhiteListed())
            {
                context.Succeed(requirement);
            }

            return Task.FromResult(0);
        }
    }
}
