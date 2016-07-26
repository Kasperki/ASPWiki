using ASPWiki.Model;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Security.Principal;

namespace ASPWiki.Services
{
    public class WikiService : IWikiService
    {
        private readonly IWikiRepository wikiRepository;

        public WikiService(IWikiRepository wikiRepository)
        {
            this.wikiRepository = wikiRepository;
        }

        public List<Node> GetWikiTree(List<WikiPage> wikiPages)
        {
            var wikiTree = new List<Node>();

            foreach (var wikiPage in wikiPages)
            {
                var node = wikiTree.Find(n => n.Name == wikiPage.PathArray[0]);

                if (node == null)
                {
                    node = new Node(wikiPage.PathArray[0]);
                    wikiTree.Add(node);
                }

                if (wikiPage.PathArray.Length == 1)
                    node.WikiPage = wikiPage;
                else
                {
                    BuildTreeNode(node, wikiPage, 1);
                }
            }

            return wikiTree;
        }

        private void BuildTreeNode(Node node, WikiPage wikiPage, int i)
        {
            int index = node.ChildNode.FindIndex(n => n.Name == wikiPage.PathArray[i]);

            if (index == -1)
            {
                node.ChildNode.Add(new Node(wikiPage.PathArray[i]));
                index = 0;
            }

            if (i == wikiPage.PathArray.Length - 1)
                node.ChildNode.Find(n => n.Name == wikiPage.PathArray[i]).WikiPage = wikiPage;
            else
            {
                BuildTreeNode(node.ChildNode[index], wikiPage, ++i);
            }
        }

        //TODO GetByPath is heavy?
        //Should keep repo organized in treeshape by path?
        //CHANGING WIKIPAGE TO ITSELFS CHILD WOULD NOT BE VALID
        //CHANGING WIKIPAGES PATH THAT HAS CHILDS WOULD NOT BE VALID OR WOULD CHANGE childs paths as well?
        public bool IsValidPath(string path, Guid id)
        {
            if (path.IndexOf('/') != -1)
            {
                string parentPath = path.Substring(0, path.LastIndexOf('/'));
                if (wikiRepository.GetByPath(parentPath) == null)
                    throw new Exception("Parent not found");
            }

            WikiPage wikiPage = wikiRepository.GetByPath(path);

            if (wikiPage != null && wikiPage.Id != id)
            {
                throw new Exception("Path already exists");
            }
            else
            {
                return true;
            }
        }

        public void Save(WikiPage wikiPage, IIdentity indetity)
        {
            wikiPage.SetPath(wikiPage.Path);
            IsValidPath(wikiPage.Path, wikiPage.Id);

            wikiPage.ContentHistory = wikiRepository.GetById(wikiPage.Id)?.ContentHistory ?? new List<string>();

            if (wikiPage.ContentHistory.Count == 0 || !String.Equals(wikiPage.ContentHistory.Last(), wikiPage.Content))
                wikiPage.ContentHistory.Add(wikiPage.Content);

            if (indetity.Name != null && indetity.Name != String.Empty)
                wikiPage.Author = indetity.Name;

            if (!indetity.IsAuthenticated)
                wikiPage.Public = true;

            wikiPage.LastModified = DateTime.Now;
            wikiRepository.Save(wikiPage);
        }

        public List<WikiPage> FilterPublic(List<WikiPage> wikiPages)
        {
            return (from entry in wikiPages where entry.Public == true select entry).ToList();
        }
    }
}
