using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Twitter;
using System.Text.Encodings.Web;

namespace ASPWiki.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public AuthenticationService(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public Task OnCreateTicket(TwitterCreatingTicketContext context)
        {
            var userId = context.UserId; //TODO GET - 26841155
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
    }
}
