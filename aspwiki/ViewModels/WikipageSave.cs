using ASPWiki.Model;
using System;
using System.Collections.Generic;

namespace ASPWiki.ViewModels
{
    public class WikipageSave
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Path { get; set; }

        public Label Label { get; set; }

        public string Content { get; set; }

        public List<Attachment> Attachments { get; set; }

        public bool Public { get; set; }
    }
}
