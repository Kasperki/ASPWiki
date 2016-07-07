using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ASPWiki.Services;

namespace ASPWiki.Controllers
{
    public class WikiController : Controller
    {
        private readonly IRouteGenerator routeGenerator;

        public WikiController(IRouteGenerator routeGenerator)
        {
            this.routeGenerator = routeGenerator;
        }

        public IActionResult Add()
        {
            string route = routeGenerator.GenerateRoute();

            return RedirectToAction("Edit", "Wiki", new { title = route });
        }

        [HttpGet("Wiki/Edit/{title}")]
        public IActionResult Edit(string title)
        {
            return View("Edit", title);
        }

        //Save

        //Load
    }
}
