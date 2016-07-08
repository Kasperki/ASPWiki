using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ASPWiki.Model
{
    public class WikiPage
    {
        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        public string Content { get; set; }

        public WikiPage() { }

        public WikiPage(string title)
        {
            this.Title = title;
        }

        public override string ToString()
        {
            return "Title: " + Title + " Content: " + Content;
        }
    }
}
