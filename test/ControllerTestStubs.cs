using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ASPWiki.Tests
{
    public class AuthorizeStub : IAuthorizationService
    {
        bool result;

        /// <summary>
        /// Stub for authorization in controller tests.
        /// </summary>
        /// <param name="result">If true, AuthorizeAsync will pass, false it will not</param>
        public AuthorizeStub(bool result)
        {
            this.result = result;
        }

        public Task<bool> AuthorizeAsync(ClaimsPrincipal user, object resource, string policyName)
        {
            return Task.FromResult(result);
        }

        public Task<bool> AuthorizeAsync(ClaimsPrincipal user, object resource, IEnumerable<IAuthorizationRequirement> requirements)
        {
            return Task.FromResult(result);
        }
    }
}
