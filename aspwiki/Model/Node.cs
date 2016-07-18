using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASPWiki.Model
{
    public class Node
    {
        public string Name;
        public List<Node> ChildNode;
        public WikiPage WikiPage;

        public Node(string name)
        {
            Name = name;
            ChildNode = new List<Node>();
        }
    }
}
