using ASPWiki.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ASPWiki.Services
{
    public class AuthenticationService : IAuthenticationService
    {  
        private HttpClient client;

        private const string validationUrl = "http://127.0.0.1:8081/test";

        public AuthenticationService()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri(validationUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<bool> ValidateToken(HttpContext contxt, string authToken, string sessionId)
        {
            HttpResponseMessage responseMessage = await client.GetAsync(validationUrl + "?authToken=" + authToken + "&sessionId=" + sessionId);

            if (responseMessage.IsSuccessStatusCode)
            {
                var responseData = responseMessage.Content.ReadAsStringAsync().Result;

                if (responseData == "NotValid")
                {
                    return false;
                }
                else
                {
                    Session session = JsonConvert.DeserializeObject<Session>(responseData);
                    await CreateClaim(contxt, session);
                }
            }

            return true;
        }

        private async Task CreateClaim(HttpContext contxt, Session session)
        {
            const string Issuer = "https://contoso.com";
            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, session.Username, ClaimValueTypes.String, Issuer));
            var userIdentity = new ClaimsIdentity("SuperSecureLogin");
            userIdentity.AddClaims(claims);
            var userPrincipal = new ClaimsPrincipal(userIdentity);

            await contxt.Authentication.SignInAsync(Constants.AuthenticationScheme, userPrincipal,
            new AuthenticationProperties
            {
                ExpiresUtc = session.Expires,
                IsPersistent = false,
                AllowRefresh = false
            });
        }
    }
}
