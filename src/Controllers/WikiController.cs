using Microsoft.AspNetCore.Mvc;
using ASPWiki.Services;
using ASPWiki.Model;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;

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
            wikiPage.LastModified = DateTime.Now;

            string parent = wikiPage.Path[0];
            wikiPage.Path = new List<string>(wikiRepository.Get(parent).Path);
            wikiPage.Path.Add(parent);

            wikiRepository.Save(wikiPage.Title, wikiPage);

            this.FlashMessageSuccess("Wikipage: " + wikiPage.Title + " succesfully saved");
            return RedirectToAction("View", "Wiki", new { title = wikiPage.Title });
        }

        [HttpPost("Wiki/IsValidPath")]
        public IActionResult IsValidPath([FromBody]string path)
        {
            var wikiPage = wikiRepository.Get(path);

            if (wikiPage == null)
                return new OkObjectResult(JsonConvert.SerializeObject("NOTVALID"));
            else
                return new OkObjectResult(JsonConvert.SerializeObject(wikiPage.Path));
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
