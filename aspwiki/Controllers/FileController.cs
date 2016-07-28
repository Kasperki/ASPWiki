using ASPWiki.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace ASPWiki.Controllers
{
    public class FileController : Controller
    {

        private readonly IWikiRepository wikiRepository;
        private readonly IAuthorizationService authorizationService;

        public FileController(IWikiRepository wikiRepository, IAuthorizationService authorizationService)
        {
            this.wikiRepository = wikiRepository;
            this.authorizationService = authorizationService;
        }

        [HttpGet("Wiki/File/View/{wikiPageId}/{id}")]
        public async Task<IActionResult> GetFile(string wikiPageId, string id)
        {
            try
            {
                var wikiPage = wikiRepository.GetById(new Guid(wikiPageId));

                if (wikiPage != null && await authorizationService.AuthorizeAsync(User, wikiPage, new WikiPageEditRequirement()))
                {
                    var fileToRetrieve = wikiPage.GetAttacment(new Guid(id));
                    return File(fileToRetrieve.Content, fileToRetrieve.ContentType, fileToRetrieve.FileName);
                }
            }
            catch (Exception)
            {
                return BadRequest();
            }

            return Forbid();
        }

        [Authorize]
        [HttpPost("Wiki/File/Delete")]
        public IActionResult DeleteFile([FromBody] object ajaxData)
        {
            JObject data = ajaxData as JObject;

            var WikiId = data.Value<string>("WikiId");
            var Id = data.Value<string>("Id");

            try
            {
                var wikiPage = wikiRepository.GetById(new Guid(WikiId));
                wikiPage.RemoveAttacment(new Guid(Id));
                return Ok();
            }
            catch (Exception)
            {
                return new BadRequestResult();
            }
        }
    }
}
