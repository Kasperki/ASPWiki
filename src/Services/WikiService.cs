using ASPWiki.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASPWiki.Services
{
    public class WikiService : IWikiService
    {
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
    }
}
