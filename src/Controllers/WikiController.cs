using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ASPWiki.Services;
using ASPWiki.Model;

namespace ASPWiki.Controllers
{
    public class WikiController : Controller
    {
        private readonly IRouteGenerator routeGenerator;

        //TODO THIS WILL BE A REAL REPOSITORY_____ !!
        private readonly IWikiRepository wikiRepository;

        public WikiController(IRouteGenerator routeGenerator, IWikiRepository wikiRepository)
        {
            this.routeGenerator = routeGenerator;
            this.wikiRepository = wikiRepository;
        }

        [HttpGet("Wiki/New/")]
        public IActionResult New(string title)
        {
            string route = routeGenerator.GenerateRoute();
            return RedirectToAction("Add", new { title = route });
        }

        [HttpGet("Wiki/Add/{title}")]
        public IActionResult Add(string title)
        {
            var wikiPage = new WikiPage(title);
            return View("Edit", wikiPage);
        }

        [HttpGet("Wiki/Edit/{title}")]
        public IActionResult Edit(string title)
        {
            var wikiPage = wikiRepository.Get(title);

            if (wikiPage == null)
                return RedirectToAction("Add", new { title = title }); 

            return View("Edit", wikiPage);
        }

        [HttpGet("Wiki/View/{title}")]
        new public IActionResult View(string title)
        {
            var wikiPage = wikiRepository.Get(title);

            if (wikiPage == null)
                return RedirectToAction("NotFound", "Wiki", new { title = title });

            return View("View", wikiPage);
        }

        [HttpPost("Wiki/Save")]
        [ValidateAntiForgeryToken]
        public IActionResult Save(WikiPage wikiPage)
        {
            System.Diagnostics.Debug.WriteLine(wikiPage.ToString());
            wikiRepository.Save(wikiPage.Title, wikiPage);

            this.FlashMessageSuccess("Wikipage: " + wikiPage.Title + " succesfully saved");
            return RedirectToAction("View", "Wiki", new { title = wikiPage.Title });
        }

        [HttpGet("Wiki/Delete/{title}")]
        public IActionResult Delete(string title)
        {
            wikiRepository.Delete(title);

            this.FlashMessageError("Wikipage: " + title + " deleted"); //TODO ADD UNDO.
            return RedirectToAction("Index", "Home");
        }

        [HttpGet("Wiki/NotFound/{title}")]
        public IActionResult NotFound(string title)
        {
            return View("NotFound", title);
        }
    }
}
