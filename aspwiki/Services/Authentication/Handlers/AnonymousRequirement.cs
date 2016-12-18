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
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            AnonymousRequirement requirement,
            WikiPage resource)
        {
            if (resource.Public == true || context.User.Identity.IsAuthenticated)
            {
                context.Succeed(requirement);
            }

            return Task.FromResult(0);
        }
    }
}
