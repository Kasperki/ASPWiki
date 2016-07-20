using System;
using System.Collections.Generic;
using ASPWiki.Model;
using System.IO;
using Newtonsoft.Json;

namespace ASPWiki.Services
{
    public class WikiPageGenerator : IGarbageGenerator<WikiPage>
    {
        private Random random;

        private readonly IWikiRepository wikiRepo;

        public WikiPageGenerator(IWikiRepository wikiRepo)
        {
            random = new Random();
            this.wikiRepo = wikiRepo;
        }

        public void GenerateToDatabase(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var wikiPage = Generate();
                wikiRepo.Save(wikiPage);
            }
        }

        public WikiPage Generate()
        {
            List<string> nouns;
            using (StreamReader sr = new StreamReader(File.OpenRead("Resources/nouns.json")))
            {
                nouns = JsonConvert.DeserializeObject<List<string>>(sr.ReadToEnd());
            }
            WikiPage wikiPage = new WikiPage(nouns[random.Next(0, nouns.Count)]);

            DateTime start = new DateTime(2010, 1, 1);
            wikiPage.LastModified = start.AddDays(random.Next(0, (DateTime.Today - start).Days));
            wikiPage.LastModified = wikiPage.LastModified.AddSeconds(random.Next(0, 80000));

            wikiPage.Content += "<h2>" + nouns[random.Next(0, nouns.Count)] + "</h2> <p>";

            for (int i = 0; i < 40; i++)
                wikiPage.Content += nouns[random.Next(0, nouns.Count)] + " ";

            wikiPage.Content += "</p>";

            if (random.Next(0, 100) > 40 && wikiRepo.GetAll().Count > 0)
            {
                wikiPage.SetPath(GetRandomWikiPage().Path);
            }

            wikiPage.label = (Label)random.Next(0, Enum.GetNames(typeof(Label)).Length);

            var versions = random.Next(1, 12);
            for (int i = 0; i < versions; i++)
            {
                if (i != versions - 1)
                    wikiPage.ContentHistory.Add(i.ToString());
                else
                    wikiPage.ContentHistory.Add(wikiPage.Content);
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

        private WikiPage GetRandomWikiPage()
        {
            return wikiRepo.GetAll()[random.Next(0, wikiRepo.GetAll().Count - 1)];
        }
    }
}
 