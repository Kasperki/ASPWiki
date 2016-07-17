using ASPWiki.Model;
using System.Collections.Generic;
using System;
using System.Linq;

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
        //CHANGING WIKIPAGE TO ITSELFS CHILD WOULD NOT BE VALID
        //CHANGING WIKIPAGES PATH THAT HAS CHILDS WOULD NOT BE VALID OR WOULD CHANGE childs paths as well?
        public bool IsValidPath(string[] path, Guid id)
        {
            if (path.Length > 1)
            {
                if (wikiRepository.GetByPath(path.Take(path.Length - 1).ToArray()) == null)
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

        public void Save(WikiPage wikiPage)
        {
            //Path comes from form in path/as/url: 
            //TODO Set as so also to model; and Path: get; parsed from that. makes this simpler
            if (wikiPage.Path[0] != null)
            {
                string[] pathValue = wikiPage.Path[0].Split('/');
                wikiPage.Path = new List<string>(pathValue);
            }
            else
            {
                wikiPage.Path = new List<string>();
            }

            wikiPage.SetPath(wikiPage.Path);
            IsValidPath(wikiPage.Path.ToArray(), wikiPage.Id);

            wikiPage.LastModified = DateTime.Now;

            wikiRepository.Save(wikiPage);
        }
    }
}
