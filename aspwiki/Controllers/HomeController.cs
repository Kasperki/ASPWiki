using Microsoft.AspNetCore.Mvc;
using ASPWiki.Services;
using System.Collections.Generic;
using AutoMapper;
using ASPWiki.ViewModels;

namespace ASPWiki.Controllers
{    
    public class HomeController : Controller
    {
        private readonly IWikiRepository wikiRepository;
        private readonly IWikiService wikiService;
        private readonly IMapper mapper;

        public HomeController(IMapper mapper, IWikiRepository wikiRepository, IWikiService wikiService)
        {
            this.wikiRepository = wikiRepository;
            this.wikiService = wikiService;
            this.mapper = mapper;
        }

        public IActionResult Index()
        {
            var wiki = wikiRepository.GetAll();
            var wikiPagesLatests = wikiRepository.GetLatest(5);
            var wikiPagesPopular = wikiRepository.GetPopular(5);

            return View("Index", new HomeViewModel() {
                LatestWikipages = mapper.Map<List<WikipageSummary>>(wikiPagesLatests),
                PopularWikipages = mapper.Map<List<WikipageSummary>>(wikiPagesPopular),
            });
        }

        public IActionResult GetAsideWikiPages()
        {
            var wikiPages = wikiRepository.GetAll();
            var wikiTree = wikiService.GetWikiTree(wikiPages);

            return new JsonResult(mapper.Map<List<NodeJsonModel>>(wikiTree));
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
