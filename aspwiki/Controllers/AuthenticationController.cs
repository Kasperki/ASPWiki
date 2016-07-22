using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ASPWiki.Services;

namespace ASPWiki.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly IAuthenticationService authenticationService;
        public AuthenticationController(IAuthenticationService authenticationService)
        {
            this.authenticationService = authenticationService;
        }

        [HttpGet("Login")]
        public async Task<IActionResult> Login()
        {
            string authToken = HttpContext.Request.Cookies["authToken"];
            string sessionId = HttpContext.Request.Cookies["sessionId"];

            if (authToken == null || sessionId == null)
                return Redirect("http://127.0.0.1:8081/login");

            bool isValid = await authenticationService.ValidateToken(HttpContext, authToken, sessionId);

            if (!isValid)
            {
                this.FlashMessageError("401");
                return View("Error");
            }

            this.FlashMessageSuccess("Welcome: " + User.Identity.Name);
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
