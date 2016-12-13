using System;
using System.Collections.Generic;
using ASPWiki.Model;

namespace ASPWiki.Services.Generators
{
    public class WikiPageGenerator : GeneratorBase, IGarbageGenerator<WikiPage>
    {
        private readonly IGarbageGenerator<Attachment> attachmentGenerator;

        public WikiPageGenerator(IGarbageGenerator<Attachment> attachmentGenerator)
        {
            this.attachmentGenerator = attachmentGenerator;
        }

        public WikiPage Generate()
        {
            WikiPage wikiPage = new WikiPage(GetRandomName());
            wikiPage.LastModified = GetRandomDateTimeBetween(new DateTime(2015, 1, 1));

            wikiPage.Content += "<h2>" + GetRandomName() + "</h2> <p>";

            for (int i = 0; i < 40; i++)
            {
                wikiPage.Content += GetRandomName() + " ";
            }

            wikiPage.Content += "</p>";

            wikiPage.Public = true;

            if (random.Next(0, 10) > 5)
            {
                wikiPage.Public = false;
                wikiPage.Author = GetRandomName();
            }

            wikiPage.Label = (Label)random.Next(0, Enum.GetNames(typeof(Label)).Length);

            var versions = random.Next(1, 12);
            for (int i = 0; i < versions; i++)
            {
                wikiPage.ContentHistory.Add(wikiPage.Content.Substring(0, wikiPage.Content.Length / (versions - i)));
            }

            wikiPage.Visits = random.Next(0, 10);
            wikiPage.Attachments = attachmentGenerator.GenerateList(random.Next(0, 5));

            foreach (var attachment in wikiPage.Attachments)
            {
                attachment.WikipageId = wikiPage.Id;
            }

            return wikiPage;
        }

        public List<WikiPage> GenerateList(int count)
        {
            List<WikiPage> list = new List<WikiPage>();

            for (int i = 0; i < count; i++)
            {
                list.Add(Generate());
            }

            return list;
        }
    }
}
 