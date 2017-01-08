using ASPWiki.Model;
using ASPWiki.Model.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ASPWiki.ViewModels
{
    public class WikipageView
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Path { get; set; }

        public bool Public { get; set; }

        [UIHint("Label")]
        public Label Label { get; set; }

        public List<Attachment> Attachments { get; set; }

        public string Size { get; set; }

        public string Author { get; set; }

        public string Content { get; set; }

        public List<string>  ContentHistory { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime LastModified { get; set; }
    }
}
