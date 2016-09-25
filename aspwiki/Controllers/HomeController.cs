using Microsoft.AspNetCore.Mvc;
using ASPWiki.Services;
using Newtonsoft.Json;
using System.Collections.Generic;
using AutoMapper;
using ASPWiki.ViewModels;

namespace ASPWiki.Controllers
{    
    //CLEAN CONTROLLERS
     //CLEAN SERVICES
     //CLEAN WIKIMODEL
    //CONSTANTS T4

    //TODO ADD LIMIT FOR ADDING NEW PAGES?
     //TODO REMOVE ALL OLD FILES

     //Mobile friendly, colors, code tag easily available

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
            return View("Index", new List<List<WikipageSummary>>{ mapper.Map<List<WikipageSummary>>(wikiPagesLatests), mapper.Map<List<WikipageSummary>>(wikiPagesPopular) });
        }

        public IActionResult GetAsideWikiPages()
        {
            var wikiPages = wikiRepository.GetAll();
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
