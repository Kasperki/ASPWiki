using ASPWiki.Model;
using ASPWiki.Model.Types;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Security.Principal;

namespace ASPWiki.Services
{
    public interface IWikiService
    {
        void Add(WikiPage wikiPage, IEnumerable<IFormFile> uploads, IIdentity indentity);
        void Update(WikiPage wikiPage, IEnumerable<IFormFile> uploads, IIdentity indentity);
        TimeSpan? GetDueDateTimeSpan(DueDate date);
        string GetVersionContent(WikiPage wikipage, string version);
        void AddVisit(WikiPage wikiPage);
        List<Node> GetWikiTree(List<WikiPage> wikiPages);
        bool IsValidPath(string path, Guid id);
    }
}
