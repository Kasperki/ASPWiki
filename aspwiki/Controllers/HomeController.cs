using Microsoft.AspNetCore.Mvc;
using ASPWiki.Services;
using Newtonsoft.Json;
using System.Collections.Generic;
using ASPWiki.Model;

namespace ASPWiki.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWikiRepository wikiRepository;
        private readonly IWikiService wikiService;

        public HomeController(IWikiRepository wikiRepository, IWikiService wikiService)
        {
            this.wikiRepository = wikiRepository;
            this.wikiService = wikiService;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            var wikiPagesLatests = wikiRepository.GetLatest(5, User.Identity.IsAuthenticated);
            var wikiPagesPopular = wikiRepository.GetPopular(5, User.Identity.IsAuthenticated);
            return View("Index", new List<List<WikiPage>>{ wikiPagesLatests, wikiPagesPopular });
        }

        public IActionResult GetAsideWikiPages()
        {
            var wikiPages = wikiRepository.GetAll(User.Identity.IsAuthenticated);
            var wikiTree = wikiService.GetWikiTree(wikiPages);

            Response.ContentType = "application/json";
            var jsonTree = JsonConvert.SerializeObject(wikiTree);
            return new OkObjectResult(jsonTree);
        }


        [HttpGet("Wiki/Error/{statusCode?}")]
        public IActionResult Error(int statusCode)
        {
            if (statusCode == 404)
                return View("PageNotFound");

            return View("Error");
        }

        [HttpGet("Wiki/Forbidden")]
        public IActionResult Forbidden()
        {
            return View("Forbidden");
        }
    }
}
