using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Twitter;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Text.Encodings.Web;
using System;
using Microsoft.AspNetCore.Hosting;

namespace ASPWiki.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IHostingEnvironment env;

        public AuthenticationService(IHttpContextAccessor httpContextAccessor, IHostingEnvironment env)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.env = env;
        }

        public Task OnCreateTicket(TwitterCreatingTicketContext context)
        {
            return Task.FromResult(0);
        }

        public Task OnRemoteFailure(FailureContext context)
        {
            context.Response.Redirect("/error?FailureMessage=" + UrlEncoder.Default.Encode(context.Failure.Message));
            context.HandleResponse();
            return Task.FromResult(0);
        }

        public AuthenticationProperties GetAuthenticationProperties()
        {
            return new AuthenticationProperties
            {
                ExpiresUtc = DateTime.Now.AddDays(1),
                IsPersistent = true,
                AllowRefresh = true,
                RedirectUri = Constants.LoginRedirectRoute
            };
        }

        public async Task CreateDevClaim()
        {
            const string Issuer = "https://contoso.com";
            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, "DevUser", ClaimValueTypes.String, Issuer));
            var userIdentity = new ClaimsIdentity("SuperSecureLogin");
            userIdentity.AddClaims(claims);
            var userPrincipal = new ClaimsPrincipal(userIdentity);

            await httpContextAccessor.HttpContext.Authentication.SignInAsync(Constants.AuthenticationSchemeCookies, userPrincipal, GetAuthenticationProperties());
        }

        public bool IsAuthenticated()
        {
            if (httpContextAccessor?.HttpContext?.User == null)
                return false;

            return httpContextAccessor.HttpContext.User.Identity.IsAuthenticated;
        }

        public bool IsAuthenticatedAndWhiteListed()
        {
            if (env.IsDevelopment())
            {
                return IsAuthenticated();
            }
            else
            {
                if (httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
                {
                    foreach (var claim in httpContextAccessor.HttpContext.User.Claims)
                    {
                        if (claim.Type == ClaimTypes.NameIdentifier && claim.Value == "26841155") //TODO CRAETE USER-WHITELISTED DOCUMENT TO REPO (cmd: add;delete) & QUERY FROM THERE
                        {
                            return true;
                        }

                    }
                }
            }

            return false;
        }
    }
}
