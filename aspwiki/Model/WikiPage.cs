using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ASPWiki.Model
{
    public class WikiPage
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        public string Author { get; set; }

        public bool Public { get; set; }

        public Label label { get; set; }

        public int Visits { get; set; }

        [BindNever]
        public List<string> ContentHistory { get; set; }

        public string Content { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime LastModified { get; set; }

        public string Path { get; set; }

        public List<Attachment> Attachments { get; set; }

        public string[] PathArray
        {
            get
            {
                return Path?.Split('/');
            }
        }

        public string Parent
        {
            get
            {
                string pathToParent = GetPathToParent();

                int lastIndex = pathToParent.LastIndexOf("/");

                if (lastIndex != -1)
                {
                    lastIndex++;
                    return pathToParent.Substring(lastIndex, pathToParent.Length - lastIndex);
                }

                return pathToParent;
            }
        }

        public WikiPage()
        {
            if (Id == null)
            {
                Id = Guid.NewGuid();
            }

            if (ContentHistory == null)
            {
                ContentHistory = new List<string>();
            }
        } 

        public WikiPage(string title)
        {
            Id = Guid.NewGuid();

            this.Title = title;
            Path = Title;
            ContentHistory = new List<string>();
        }

        public void SetPath(string ParentPath)
        {
            if (ParentPath != null && ParentPath != String.Empty)
            {
                Path = ParentPath + "/" + Title;
            }
            else
            {
                Path = Title;
            }
        }

        public string GetPathToParent()
        {
            if (Path == null || Path == string.Empty)
                return string.Empty;

            int lastIndex = Path.LastIndexOf("/");

            if (lastIndex == -1)
                return string.Empty;

            return Path.Substring(0, lastIndex);
        }

        private const int SUMMARY_LENGTH = 200;
        public string GetContentSummary()
        {
            if (Content == null || Content.Length <= 0)
                return string.Empty;

            int length = Content.Length < SUMMARY_LENGTH ? Content.Length : SUMMARY_LENGTH;

            Regex rgx = new Regex(@"<\/[a-zA-Z""]*>");
            var match = rgx.Match(Content.Substring(length));
            string s = Content.Substring(0, length + match.Index + match.Length);

            s = s.Replace("<h1>", "<h4>").Replace("</h1>", "</h4>");
            s = s.Replace("<h2>", "<h4>").Replace("</h2>", "</h4>");
            s = s.Replace("<h3>", "<h4>").Replace("</h3>", "</h4>");

            return s;
        }

        public string GetSizeKiloBytes()
        {
            if (Content == null)
                return "0 KB";

            return (Content.Length * sizeof(char) / 1024f).ToString("0.00", new CultureInfo("fi")) + " KB";
        }

        public override string ToString()
        {
            return "Title: " + Title + " Content: " + Content + " Path: " + Path;
        }

        public Attachment GetAttacment(Guid attacmentId)
        {
            return Attachments.Find(x => x.FileId == attacmentId);
        }

        public void RemoveAttacment(Guid attacmentId)
        {
            Attachments.RemoveAll(x => x.FileId == attacmentId);
        }
    }
}
