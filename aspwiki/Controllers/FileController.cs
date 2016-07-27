using ASPWiki.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;

namespace ASPWiki.Controllers
{
    public class FileController : Controller
    {

        private readonly IWikiRepository wikiRepository;

        public FileController(IWikiRepository wikiRepository)
        {
            this.wikiRepository = wikiRepository;
        }

        [HttpGet("Wiki/File/View/{wikiPageId}/{id}")]
        public IActionResult GetFile(string wikiPageId, string id)
        {
            var fileToRetrieve = wikiRepository.GetById(new Guid(wikiPageId)).Attachments.Find(x => x.FileId == new Guid(id));
            return File(fileToRetrieve.Content, fileToRetrieve.ContentType, fileToRetrieve.FileName);
        }

        [HttpPost("Wiki/File/Delete")]
        public IActionResult DeleteFile([FromBody] object ajaxData)
        {
            JObject data = ajaxData as JObject;

            var WikiId = data.Value<string>("WikiId");
            var Id = data.Value<string>("Id");

            var fileToRetrieve = wikiRepository.GetById(new Guid(WikiId)).Attachments.Find(x => x.FileId == new Guid(Id));
            wikiRepository.GetById(new Guid(WikiId)).Attachments.Remove(fileToRetrieve);
            return new OkObjectResult("");
        }
    }
}
