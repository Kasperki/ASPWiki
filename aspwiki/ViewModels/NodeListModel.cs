using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASPWiki.ViewModels
{
    public class NodeListModel
    {
        public string Name;
        public List<NodeListModel> ChildNode;
        public WikipageSummary WikiPage;
    }
}
