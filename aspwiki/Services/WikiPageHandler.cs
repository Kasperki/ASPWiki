using ASPWiki.Model;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace ASPWiki.Services
{
    public class WikiPageEditRequirement : IAuthorizationRequirement
    {
    }

    public class WikiPageHandler : AuthorizationHandler<WikiPageEditRequirement, WikiPage>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            WikiPageEditRequirement requirement,
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
