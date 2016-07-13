using Microsoft.AspNetCore.Mvc;
using ASPWiki.Services;
using Newtonsoft.Json;
using ASPWiki.Model;
using System.Collections.Generic;

namespace ASPWiki.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWikiRepository wikiRepository;

        public HomeController(IWikiRepository wikiRepository)
        {
            this.wikiRepository = wikiRepository;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            var wikiPages = wikiRepository.GetLatest(5);
            return View("Index", wikiPages);
        }

        public IActionResult GetAsideWikiPages()
        {
            var wikiPages = wikiRepository.GetAll();
            wikiPages.Sort((emp1, emp2) => emp1.Path.Count.CompareTo(emp2.Path.Count));

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
                    KEK(node, wikiPage, 1);
                }
            }

            Response.ContentType = "application/json";
            var jsonTree = JsonConvert.SerializeObject(wikiTree);
            return new OkObjectResult(jsonTree);
        }

        private void KEK(Node node, WikiPage wikiPage, int i)
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
                KEK(node.ChildNode[index], wikiPage, ++i);
            }
        }

        public class Node
        {
            public string Name;
            public List<Node> ChildNode;
            public WikiPage WikiPage;

            public Node (string n)
            {
                Name = n;
                ChildNode = new List<Node>();
            }
        }
    }
}
