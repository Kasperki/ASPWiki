using ASPWiki.Model;

namespace ASPWiki.Services.Generators
{
    public class TestDataGenerator : GeneratorBase
    {
        private const int WIKIPAGECOUNT = 15;

        public TestDataGenerator(IDatabaseConnection databaseConnection, IWikiRepository wikiRepo, IGarbageGenerator<WikiPage> wikiPageGenerator)
        {
            var database = databaseConnection.GetDatabase();
            database.DropCollection(Constants.WikiPagesCollectionName);

            var wikipages = wikiPageGenerator.GenerateList(WIKIPAGECOUNT);

            foreach (var item in wikipages)
            {
                if (GetRandomBoolean(30))
                {
                    item.SetPath(wikipages[random.Next(0, wikipages.Count)].Path);
                }
                
            }

            foreach (var wikipage in wikipages)
            {
                wikiRepo.Add(wikipage);
            }
        }
    }
}
