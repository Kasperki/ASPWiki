using ASPWiki.Model;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Security.Principal;

namespace ASPWiki.Services
{
    public interface IWikiService
    {
        void Save(WikiPage wikiPage, IEnumerable<IFormFile> uploads, IIdentity indentity);
        List<Node> GetWikiTree(List<WikiPage> wikiPages);
        bool IsValidPath(string path, Guid id);
        List<WikiPage> FilterPublic(List<WikiPage> wikiPages);
    }
}
