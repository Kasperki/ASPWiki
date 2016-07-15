using Microsoft.AspNetCore.Mvc;
using ASPWiki.Services;
using ASPWiki.Model;
using System;
using Newtonsoft.Json;
using System.Linq;

namespace ASPWiki.Controllers
{
    //DRY - @PARAMETIRIZE
    //MINIMIZE CONTROLLER LOGIC - UNIT TESTING - 
    public class WikiController : Controller
    {
        private readonly IRouteGenerator routeGenerator;

        //TODO THIS WILL BE A REAL REPOSITORY_____ !!
        private readonly IWikiRepository wikiRepository;

        private readonly IWikiService wikiService;

        public WikiController(IRouteGenerator routeGenerator, IWikiRepository wikiRepository, IWikiService wikiService)
        {
            this.routeGenerator = routeGenerator;
            this.wikiRepository = wikiRepository;
            this.wikiService = wikiService;
        }

        [HttpGet("Wiki/New")]
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
            if (ModelState.IsValid)
            {
                try
                {
                    wikiService.Save(wikiPage);
                    this.FlashMessageSuccess("Wikipage: " + wikiPage.Title + " succesfully saved");
                    return Redirect("/Wiki/View/" + wikiPage.GetPathString());
                }
                catch (Exception e)
                {
                    this.FlashMessageError(e.Message);
                    return View("Edit", wikiPage);
                }
            }
            else
            {
                this.FlashMessageError("Model invalid");
                return View("Edit", wikiPage);
            }
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

        [HttpPost("Wiki/IsValidPath")]
        public IActionResult IsValidPath([FromBody]string[] ajaxData)
        {
            string pathValue = ajaxData[0];
            string title = ajaxData[1];

            try
            {
                wikiService.IsValidPath(pathValue, title);

                if (pathValue != String.Empty)
                    return new OkObjectResult(JsonConvert.SerializeObject(wikiRepository.Get(pathValue).Path));
                else 
                    return new OkObjectResult(JsonConvert.SerializeObject(new string[] { "" }));
            }
            catch (Exception e)
            {
                return new OkObjectResult(JsonConvert.SerializeObject(e.Message));
            }   
        }

        private string[] GetParsedPath()
        {
            var path = (string)RouteData.Values.Values.First();
            return path?.Split('/');
        }
    }
}
