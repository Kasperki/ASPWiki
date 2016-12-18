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
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            AuthenticationRequirement requirement,
            WikiPage resource)
        {
            if (context.User.Identity.IsAuthenticated)
            {
                context.Succeed(requirement);
            }

            return Task.FromResult(0);
        }
    }
}
