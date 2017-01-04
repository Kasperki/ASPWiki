using ASPWiki.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASPWiki.ViewModels
{
    public class NodeJsonModel
    {
        public string Name;
        public List<NodeJsonModel> ChildNode;
        public WikiPageMetadata WikiPage;
    }
}
