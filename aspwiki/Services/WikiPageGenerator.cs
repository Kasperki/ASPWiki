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

        private List<string> nouns;

        private readonly IWikiRepository wikiRepo;

        public WikiPageGenerator(IWikiRepository wikiRepo)
        {
            random = new Random();

            using (StreamReader sr = new StreamReader(File.OpenRead("Resources/nouns.json")))
            {
                nouns = JsonConvert.DeserializeObject<List<string>>(sr.ReadToEnd());
            }

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
            WikiPage wikiPage = new WikiPage(nouns[random.Next(0, nouns.Count)]);

            DateTime start = new DateTime(2010, 1, 1);
            wikiPage.LastModified = start.AddDays(random.Next(0, (DateTime.Today - start).Days));
            wikiPage.LastModified = wikiPage.LastModified.AddSeconds(random.Next(0, 80000));

            wikiPage.Content += "<h2>" + nouns[random.Next(0, nouns.Count)] + "</h2> <p>";

            for (int i = 0; i < 40; i++)
                wikiPage.Content += nouns[random.Next(0, nouns.Count)] + " ";

            wikiPage.Content += "</p>";

            wikiPage.Public = true;

            if (random.Next(0, 10) > 5)
            {
                wikiPage.Public = false;
                wikiPage.Author = nouns[random.Next(0, nouns.Count)];
            }

            if (random.Next(0, 100) > 40 && wikiRepo.GetAll(!wikiPage.Public).Count > 0)
            {
                wikiPage.SetPath(GetRandomWikiPage(!wikiPage.Public).Path);
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

            wikiPage.Attachments = new List<Attachment>();

            var attachmentCount = random.Next(0, 5);
            for (int i = 0; i < attachmentCount; i++)
            {
                wikiPage.Attachments.Add(GenerateAttachment());
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

        private WikiPage GetRandomWikiPage(bool authenticated)
        {
            return wikiRepo.GetAll(authenticated)[random.Next(0, wikiRepo.GetAll(authenticated).Count - 1)];
        }

        private Attachment GenerateAttachment()
        {
            Attachment attachment = new Attachment();
            attachment.FileId = Guid.NewGuid();
            attachment.FileName = nouns[random.Next(0, nouns.Count)];

            attachment.Content = ConvertStringToByteArray("CONTENT");
            attachment.ContentType = "text/plain";

            return attachment;
        }

        private byte[] ConvertStringToByteArray(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;   
        } 
    }
}
 