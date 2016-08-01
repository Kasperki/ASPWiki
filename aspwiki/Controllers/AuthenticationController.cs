using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ASPWiki.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using ASPWiki.Model;
using System;

namespace ASPWiki.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly IAuthenticationService authenticationService;
        private readonly IHostingEnvironment env;
        private readonly ILogger<AuthenticationController> logger;

        public AuthenticationController(IAuthenticationService authenticationService, IHostingEnvironment env, ILogger<AuthenticationController> logger)
        {
            this.authenticationService = authenticationService;
            this.env = env;
            this.logger = logger;
        }

        [HttpGet("Login")]
        public async Task<IActionResult> Login()
        {
            Session session;

            if (env.IsProduction())
            {
                string authToken = HttpContext.Request.Cookies["authToken"];
                string sessionId = HttpContext.Request.Cookies["sessionId"];

                if (authToken == null || sessionId == null)
                    return Redirect("https://127.0.0.1:8081/login");

                try
                {
                    session = await authenticationService.ValidateToken(authToken, sessionId);
                }
                catch (Exception e)
                {
                    logger.LogWarning(new EventId(100), e, "Login failed from ip:" + HttpContext.Connection.RemoteIpAddress.MapToIPv4());

                    this.FlashMessageError("401");
                    return View("Error");
                }
            }
            else
            {
                session = await authenticationService.CreateDevSession();
            }

            logger.LogInformation(session.Username + " logged in");
            this.FlashMessageSuccess("Welcome: " + session.Username + "!");
            return Redirect("/");
        }

        [HttpGet("Logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.Authentication.SignOutAsync(Constants.AuthenticationScheme);
            this.FlashMessageSuccess("Logged out succesfully!");

            return Redirect("/");
        }
    }
}
