using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ASPWiki.Services;
using ASPWiki.Model;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Security.Claims;

namespace ASPWiki.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly IAuthenticationService authenticationService;
        private readonly IHostingEnvironment env;

        public AuthenticationController(IAuthenticationService authenticationService, IHostingEnvironment env)
        {
            this.authenticationService = authenticationService;
            this.env = env;
        }

        [HttpGet("Login")]
        public async Task<IActionResult> Login()
        {
            bool isValid = false;

            if (env.IsProduction())
            {
                string authToken = HttpContext.Request.Cookies["authToken"];
                string sessionId = HttpContext.Request.Cookies["sessionId"];

                if (authToken == null || sessionId == null)
                    return Redirect("https://127.0.0.1:8081/login");

                isValid = await authenticationService.ValidateToken(authToken, sessionId);
            }
            else
            {
                isValid = await authenticationService.CreateDevSession();
            }
                

            if (!isValid)
            {
                this.FlashMessageError("401");
                return View("Error");
            }

            this.FlashMessageSuccess("Welcome!");
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
