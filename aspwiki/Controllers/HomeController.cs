using Microsoft.AspNetCore.Mvc;
using ASPWiki.Services;
using Newtonsoft.Json;

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
            var wikiPages = wikiRepository.GetLatest(5);
            return View("Index", wikiPages);
        }

        public IActionResult GetAsideWikiPages()
        {
            var wikiPages = wikiRepository.GetAll();
            var wikiTree = wikiService.GetWikiTree(wikiPages);

            Response.ContentType = "application/json";
            var jsonTree = JsonConvert.SerializeObject(wikiTree);
            return new OkObjectResult(jsonTree);
        }
    }
}
