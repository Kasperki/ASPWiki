using Microsoft.AspNetCore.Mvc;
using ASPWiki.Services;
using ASPWiki.Model;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using AutoMapper;
using ASPWiki.ViewModels;

namespace ASPWiki.Controllers
{
    /// <summary>
    /// WikiController
    /// 
    /// TODO CONTENT HISTORY GET BY AJAX REQUEST, DO NOT SEND WHOLE DATA TO VIEW!
    /// </summary>
    public class WikiController : Controller
    {
        private readonly IRouteGenerator routeGenerator;

        private readonly IWikiRepository wikiRepository;

        private readonly IWikiService wikiService;

        private readonly IAuthorizationService authorizationService;

        private readonly ILogger<WikiController> logger;

        private readonly IMapper mapper;

        public WikiController(IMapper mapper, IRouteGenerator routeGenerator, IWikiRepository wikiRepository, IWikiService wikiService, IAuthorizationService authorizationService, ILogger<WikiController> logger)
        {
            this.routeGenerator = routeGenerator;
            this.wikiRepository = wikiRepository;
            this.wikiService = wikiService;
            this.authorizationService = authorizationService;
            this.logger = logger;
            this.mapper = mapper;
        }

        [HttpGet("Wiki/New")]
        public IActionResult New()
        {
            string route = routeGenerator.GenerateRoute();
            return RedirectToAction("Add", new { title = route });
        }

        [HttpGet("Wiki/Add/{title?}")]
        public IActionResult Add(string title)
        {
            var wikiPage = new WikiPage(title);
            return View("Edit", mapper.Map<WikipageEdit>(wikiPage));
        }

        [HttpGet("Wiki/Edit/{*path}")]
        public async Task<IActionResult> Edit(string path)
        {
            var wikiPage = wikiRepository.GetByPath(path);

            if (wikiPage == null)
                return RedirectToAction("Add", new { title = this.GetLastItemFromPath(path) });

            if (await authorizationService.AuthorizeAsync(User, wikiPage, new WikiPageEditRequirement()))
            {
                return View("Edit", mapper.Map<WikipageEdit>(wikiPage));
            }
            else
            {
                return new ChallengeResult();
            }
        }

        [HttpGet("Wiki/View/{*path}")]
        public async Task<IActionResult> ViewPage(string path, string version)
        {
            var wikiPage = wikiRepository.GetByPath(path);

            if (wikiPage == null)
                return RedirectToAction("NotFound", "Wiki", new { title = this.GetLastItemFromPath(path) });

            int versionNum;
            if (version != null && int.TryParse(version, out versionNum))
            {
                if(versionNum >= 0 && versionNum < wikiPage.ContentHistory.Count)
                    wikiPage.Content = wikiPage.ContentHistory[Convert.ToInt32(versionNum)];
            }

            if (await authorizationService.AuthorizeAsync(User, wikiPage, new WikiPageEditRequirement()))
            {
                wikiService.AddVisit(wikiPage);
                return View("View", mapper.Map<WikipageView>(wikiPage));
            }
            else
            {
                return new ChallengeResult();
            }
        }

        [HttpPost("Wiki/Save")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(WikipageSave wikipageSave, IEnumerable<IFormFile> uploads)
        {
            if (ModelState.IsValid)
            {
                var wiki = wikiRepository.GetById(wikipageSave.Id);
                if (wiki != null)
                {
                   if (!await authorizationService.AuthorizeAsync(User, wiki, new WikiPageEditRequirement()))
                   {
                       return new ChallengeResult();
                   }
                }

                try
                {
                    WikiPage wikiPage = mapper.Map<WikiPage>(wikipageSave);

                    if (wiki == null)
                    {
                        wikiService.Add(wikiPage, uploads, User.Identity);
                    }
                    else
                    {
                        wikiService.Update(wikiPage, uploads, User.Identity);
                    }

                    logger.LogInformation(User.Identity?.Name + " saved new wikipage:" + wikiPage.Title);
                    this.FlashMessageSuccess("Wikipage: " + wikiPage.Title + " succesfully saved");
                    return Redirect("/Wiki/View/" + wikiPage.Path);
                }
                catch (Exception e)
                {
                    logger.LogError(new EventId(Constants.ERROR_CODE_EXPECTION), e, "Error saving wikipage");
                    this.FlashMessageError(e.Message); //TODO This should not print every single error
                    return View("Edit", wikipageSave);
                }
            }
            else
            {
                logger.LogError("Error saving wikipage model state not valid:" + this.GetModelStateErrors());
                this.FlashMessageError("Model invalid: " + this.GetModelStateErrors());
                return View("Edit", wikipageSave);
            }
        }

        [Authorize]
        [HttpGet("Wiki/Delete/{*path}")]
        public async Task<IActionResult> Delete(string path)
        {
            var wikiPage = wikiRepository.GetByPath(path);

            if (await authorizationService.AuthorizeAsync(User, wikiPage, new WikiPageEditRequirement()))
            {
                wikiRepository.Delete(wikiPage);

                logger.LogInformation(User?.Identity?.Name + " deleted wikipage:" + path);
                this.FlashMessageError("Wikipage: " + this.GetLastItemFromPath(path) +
                    " deleted: <a style='float:right;' href='/Wiki/Revert/" + path +
                    "'><b><i class='fa fa-reply' aria-hidden='true'></i>UNDO </b></a>");

                return RedirectToAction("Index", "Home");
            }
            else
            {
                return new ChallengeResult();
            }
        }

        [Authorize]
        [HttpGet("Wiki/Revert/{*path}")]
        public IActionResult Revert(string path)
        {
            if (wikiRepository.Recover(path))
            {
                this.FlashMessageSuccess("Wikipage: " + this.GetLastItemFromPath(path) + " recovered");
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet("Wiki/NotFound/{title?}")]
        public IActionResult NotFound(string title)
        {
            return View("NotFound", title);
        }

        [HttpPost("Wiki/IsValidPath")]
        public IActionResult IsValidPath([FromBody]object ajaxData)
        {
            JObject data = ajaxData as JObject;

            var path = data.Value<string>("Path");
            var Id = data.Value<string>("Id");

            string[] pathValue = path.Split('/');

            try
            {
                wikiService.IsValidPath(path, new Guid(Id));
                return new OkObjectResult(JsonConvert.SerializeObject(pathValue));
            }
            catch (Exception e)
            {
                return new OkObjectResult(JsonConvert.SerializeObject(e.Message));
            }   
        }

        [HttpPost("Wiki/Search")]
        public IActionResult Search([FromBody]object searchKeyword)
        {
            JObject data = searchKeyword as JObject;

            var searchData = data.Value<string>("data");

            List<WikiPage> wikipages = wikiRepository.SearchByTitle(searchData);

            Response.ContentType = "text/javascript";
            return new OkObjectResult(JsonConvert.SerializeObject(wikipages));
        }
    }
}
