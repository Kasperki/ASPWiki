using Microsoft.AspNetCore.Mvc;
using ASPWiki.Services;
using ASPWiki.Model;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace ASPWiki.Controllers
{
    //TODO
      //VALIDATE PATH ON SAVE, NO SAME PAHT NAME!!!!
      //ASIDE
      
     //ADD WIKI SERVICE :) - MINIMIZE CONTROLLER LOGIC - UNIT TESTING
      //ON DELETE WHAT TO DO TO ROUTES :D xddd? leave as is? do not allow itself on parent??

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

        [HttpGet("Wiki/Add/{title?}")]
        public IActionResult Add(string title)
        {
            var wikiPage = new WikiPage(title);
            return View("Edit", wikiPage);
        }

        [HttpGet("Wiki/Edit/{*path}")]
        public IActionResult Edit()
        {
            var paths = GetParsedPath();
            var wikiPage = wikiRepository.GetByPath(paths);

            if (wikiPage == null)
                return RedirectToAction("Add", new { title = paths?[paths.Length - 1] }); 

            return View("Edit", wikiPage);
        }

        [HttpGet("Wiki/View/{*path}")]
        new public IActionResult View()
        {
            var paths = GetParsedPath();
            var wikiPage = wikiRepository.GetByPath(paths);

            if (wikiPage == null)
                return RedirectToAction("NotFound", "Wiki", new { title = paths?[paths.Length - 1] });

            return View("View", wikiPage);
        }

        [HttpPost("Wiki/Save")]
        [ValidateAntiForgeryToken]
        public IActionResult Save(WikiPage wikiPage)
        {
            System.Diagnostics.Debug.WriteLine(wikiPage.ToString());
            wikiPage.LastModified = DateTime.Now;

            string parent = wikiPage.Path[0];

            if (parent != null)
                wikiPage.SetPath(wikiRepository.Get(parent).Path);
            else
                wikiPage.Path = new List<string>(new string[] { wikiPage.Title });

            wikiRepository.Save(wikiPage.Title, wikiPage);

            this.FlashMessageSuccess("Wikipage: " + wikiPage.Title + " succesfully saved");

            return Redirect("/Wiki/View/"+wikiPage.GetPathString());
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

        [HttpGet("Wiki/NotFound/{title?}")]
        public IActionResult NotFound(string title)
        {
            return View("NotFound", title);
        }

        private string[] GetParsedPath()
        {
            var path = (string)RouteData.Values.Values.First();
            return path?.Split('/');
        }
    }
}
