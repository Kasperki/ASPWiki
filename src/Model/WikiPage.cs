using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;

namespace ASPWiki.Model
{
    public class WikiPage
    {
        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        public string Content { get; set; }

        public DateTime LastModified { get; set; }

        public List<string> Path { get; set; }

        public string Parent
        {
            get
            {
                string parent = string.Empty;
                if (Path.Count >= 2)
                {
                    parent = Path[Path.Count - 2];
                }
                return Path.Last();
            }
        }

        public WikiPage() { }

        public WikiPage(string title)
        {
            this.Title = title;
            Path = new List<string>(new string[]{ Title });
        }

        public void SetPath(List<string> ParentPath)
        {
            Path = new List<string>(ParentPath);
            Path.Add(Title);
        }

        public string GetPathString()
        {
            string path = String.Empty;

            for (int i = 0; i < Path.Count; i++)
            {
                path += Path[i];

                if (i != Path.Count - 1)
                    path += "/";
            }

            return path;
        }

        private const int SUMMARY_LENGTH = 200;
        public string GetContentSummary()
        {
            if (Content == null || Content.Length <= 0)
                return string.Empty;

            int length = Content.Length < SUMMARY_LENGTH ? Content.Length : SUMMARY_LENGTH;

            Regex rgx = new Regex(@"<\/.*>");
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

            return (Content.Length * sizeof(char) / 1024f).ToString("0.00") + " KB";
        }

        public override string ToString()
        {
            return "Title: " + Title + " Content: " + Content;
        }
    }
}
