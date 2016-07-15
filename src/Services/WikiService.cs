using ASPWiki.Model;
using System.Collections.Generic;
using System;

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
                var node = wikiTree.Find(n => n.Name == wikiPage.Path[0]);

                if (node == null)
                {
                    node = new Node(wikiPage.Path[0]);
                    wikiTree.Add(node);
                }

                if (wikiPage.Path.Count == 1)
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
            int index = node.ChildNode.FindIndex(n => n.Name == wikiPage.Path[i]);

            if (index == -1)
            {
                node.ChildNode.Add(new Node(wikiPage.Path[i]));
                index = 0;
            }

            if (i == wikiPage.Path.Count - 1)
                node.ChildNode.Find(n => n.Name == wikiPage.Path[i]).WikiPage = wikiPage;
            else
            {
                BuildTreeNode(node.ChildNode[index], wikiPage, ++i);
            }
        }

        //TODO GetByPath is heavy?
         //Should keep repo organized in treeshape by path?
        public bool IsValidPath(string parent, string title)
        {
            if (parent == null || parent == String.Empty)
            {
                if (wikiRepository.GetByPath(new string[] { title }) != null)
                {
                    throw new Exception("Path already exists");
                }
                else
                {
                    return true;
                }
            }

            WikiPage parentPage = wikiRepository.Get(parent);

            if (parentPage == null)
                throw new Exception("Parent not found");

            List<string> path = new List<string>(parentPage.Path);
            path.Add(title);

            WikiPage wikiPage = wikiRepository.GetByPath(path.ToArray());

            if (wikiPage != null && parent != wikiRepository.Get(title).Parent)
                throw new Exception("Path already exists");

            return true;
        }

        public void Save(WikiPage wikiPage)
        {
            wikiPage.LastModified = DateTime.Now;

            string parent = wikiPage.Path[0];
            IsValidPath(parent, wikiPage.Title);

            if (parent != null)
                wikiPage.SetPath(wikiRepository.Get(parent).Path);
            else
                wikiPage.Path = new List<string>(new string[] { wikiPage.Title });

            wikiRepository.Save(wikiPage.Title, wikiPage);
        }
    }
}
