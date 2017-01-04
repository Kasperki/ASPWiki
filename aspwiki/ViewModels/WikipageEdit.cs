using ASPWiki.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ASPWiki.ViewModels
{
    public class WikipageEdit
    {
        public Guid Id { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Title { get; set; }

        public string Path { get; set; }

        public string[] PathArray { get; set; }

        public string PathToParent { get; set; }

        [UIHint("Label")]
        public Label Label { get; set; }

        public string Content { get; set; }

        public List<Attachment> Attachments { get; set; }

        public bool Public { get; set; }
    }
}
