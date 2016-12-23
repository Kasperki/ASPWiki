using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ASPWiki.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http.Authentication;

namespace ASPWiki.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly IHostingEnvironment env;
        private readonly IAuthenticationService authenticationService;
        private readonly ILogger<AuthenticationController> logger;

        public AuthenticationController(IHostingEnvironment env, ILogger<AuthenticationController> logger, IAuthenticationService authenticationService)
        {
            this.env = env;
            this.logger = logger;
            this.authenticationService = authenticationService;
        }

        [HttpGet("Login")]
        public IActionResult Login()
        {
            return Redirect(Constants.LoginRedirectRoute);
        }


        [HttpGet("Logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.Authentication.SignOutAsync(Constants.AuthenticationSchemeCookies);
            this.FlashMessageSuccess("Logged out succesfully!");

            return Redirect(Constants.LoginRedirectRoute);
        }
    }
}
