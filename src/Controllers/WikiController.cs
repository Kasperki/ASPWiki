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

        //TODO CREATE ADD ROUTE -> PAGE
         //CHECK THAT IF THE PAGE ALREADY EXISTS, | NO ERRORS!!!
         //SHOW ALERTS IF ADDED, EDITED, REMOVED, OR IT EXISTS
         //@RAZOR Partialize stuff that can be done .. after that look at repositories
        public IActionResult Add()
        {
            string route = routeGenerator.GenerateRoute();
            return RedirectToAction("Edit", "Wiki", new {title = route});
        }

        [HttpGet("Wiki/Edit/{title}")]
        public IActionResult Edit(string title)
        {
            WikiPage wikiPage;

            if (wikiRepository.Exists(title))
                wikiPage = wikiRepository.Get(title);
            else
            {
                wikiPage = new WikiPage(title);
            }

            return View("Edit", wikiPage);
        }

        [HttpGet("Wiki/View/{title}")]
        new public IActionResult View(string title)
        {
            var wikiPage = wikiRepository.Get(title);

            return View("View", wikiPage);
        }

        [HttpPost("Wiki/Save")]
        [ValidateAntiForgeryToken]
        public IActionResult Save(WikiPage wikiPage)
        {
            System.Diagnostics.Debug.WriteLine(wikiPage.ToString());
            wikiRepository.Save(wikiPage.Title, wikiPage);

            return RedirectToAction("View", "Wiki", new { title = wikiPage.Title });
        }

        [HttpGet("Wiki/Delete/{title}")]
        public IActionResult Delete(string title)
        {
            wikiRepository.Delete(title);
            return RedirectToAction("Index", "Home");
        }
    }
}
