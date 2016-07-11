using Microsoft.AspNetCore.Mvc;
using ASPWiki.Services;

namespace ASPWiki.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWikiRepository wikiRepository;

        public HomeController(IWikiRepository wikiRepository)
        {
            this.wikiRepository = wikiRepository;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            var wikiPages = wikiRepository.GetLatest(5);
            return View("Index", wikiPages);
        }
    }
}
