using ASPWiki;
using ASPWiki.Model;
using ASPWiki.Services;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using System;
using Xunit;

namespace test.Services
{
    public class WikiRepoTest
    {
        private WikiRepository wikiRepo;
        private TestDatabaseConnection testConnection;
        private IMongoCollection<WikiPage> collection;

        public WikiRepoTest()
        {
            SetUp(true);
        }

        public void SetUp(bool authenticated)
        {
            Mock<IHttpContextAccessor> httpContextMock = new Mock<IHttpContextAccessor>();
            httpContextMock.Setup(repo => repo.HttpContext.User.Identity.IsAuthenticated).Returns(authenticated);
            httpContextMock.Setup(repo => repo.HttpContext.Session.Id).Returns("sessionID");

            testConnection = new TestDatabaseConnection();
            collection = testConnection.GetDatabase().GetCollection<WikiPage>(Constants.WikiPagesCollectionName);
            collection.DeleteMany(new BsonDocument());

            wikiRepo = new WikiRepository(testConnection, httpContextMock.Object);
        }

        private bool IsWikiPageEqual(WikiPage expected, WikiPage actual)
        {
            if (expected.Id != actual.Id)
                return false;

            if (expected.Label != actual.Label)
                return false;

            if (expected.Path != actual.Path)
                return false;

            if (expected.LastModified != actual.LastModified)
                return false;

            if (expected.Visits != actual.Visits)
                return false;

            if (expected.Title != actual.Title)
                return false;

            if (expected.Content != actual.Content)
                return false;

            return true;
        }

        [Fact]
        public void GetByPath_Should_Return_WikiPage()
        {
            WikiPage wiki1 = new WikiPage() { Path = "SET" };
            WikiPage wiki2 = new WikiPage() { Path = "KEK" };

            collection.InsertOne(wiki1);
            collection.InsertOne(wiki2);

            var actual = wikiRepo.GetByPath("KEK");
            Assert.Equal(true, IsWikiPageEqual(wiki2, actual));
        }

        [Fact]
        public void GetByPath_Should_Return_Null_If_Not_Found()
        {
            WikiPage wiki1 = new WikiPage() { Path = "SET" };

            collection.InsertOne(wiki1);

            var actual = wikiRepo.GetByPath("KEK");
            Assert.Equal(null, actual);
        }

        [Fact]
        public void GetById_Should_Return_WikiPage()
        {
            WikiPage wiki1 = new WikiPage();
            WikiPage wiki2 = new WikiPage();

            collection.InsertOne(wiki1);
            collection.InsertOne(wiki2);

            var actual = wikiRepo.GetById(wiki1.Id);
            Assert.Equal(true, IsWikiPageEqual(wiki1, actual));
        }

        [Fact]
        public void GetById_Should_Return_Null_If_Not_Found()
        {
            WikiPage wiki1 = new WikiPage();
            WikiPage wiki2 = new WikiPage();

            collection.InsertOne(wiki1);
            collection.InsertOne(wiki2);

            var actual = wikiRepo.GetById(Guid.NewGuid());
            Assert.Equal(null, actual);
        }

        [Fact]
        public void GetAll_Should_Return_All_WikiPages()
        {
            int expectedWikiPageCount = 6;

            for (int i = 0; i < expectedWikiPageCount; i++)
            {
                WikiPage wiki = new WikiPage();
                collection.InsertOne(wiki);
            }

            var actual = wikiRepo.GetAll();
            Assert.Equal(expectedWikiPageCount, actual.Count);
        }

        [Fact]
        public void GetAll_Should_Return_Only_Public_WikiPages_If_Not_Authenticated()
        {
            SetUp(false);

            int expectedWikiPageCount = 3;
            for (int i = 0; i < 6; i++)
            {
                WikiPage wiki = new WikiPage();

                if (i < expectedWikiPageCount)
                {
                    wiki.Public = true;
                }

                collection.InsertOne(wiki);
            }

            var actual = wikiRepo.GetAll();
            Assert.Equal(expectedWikiPageCount, actual.Count);
        }

        [Fact]
        public void GetAll_Should_Return_null_if_no_wikipages_exists()
        {
            var actual = wikiRepo.GetAll();
            Assert.Equal(0, actual.Count);
        }


        [Fact]
        public void SearchByTitle_should_return_wikipages_with_right_title()
        {
            WikiPage wiki1 = new WikiPage("DAAC");
            WikiPage wiki2 = new WikiPage("BBA");
            WikiPage wiki3 = new WikiPage("AAB");
            WikiPage wiki4 = new WikiPage("CAA");

            collection.InsertOne(wiki1);
            collection.InsertOne(wiki2);
            collection.InsertOne(wiki3);
            collection.InsertOne(wiki4);

            var actual = wikiRepo.SearchByTitle("AA");
            Assert.Equal(3, actual.Count);
        }

        [Fact]
        public void GetPopular_Should_return_most_popular_wikipages_limited()
        {
            WikiPage wiki1 = new WikiPage() { Visits = 33 };
            WikiPage wiki2 = new WikiPage() { Visits = 12 };
            WikiPage wiki3 = new WikiPage() { Visits = 1 };
            WikiPage wiki4 = new WikiPage() { Visits = 66 };

            collection.InsertOne(wiki1);
            collection.InsertOne(wiki2);
            collection.InsertOne(wiki3);
            collection.InsertOne(wiki4);

            var actual = wikiRepo.GetPopular(2);
            Assert.Equal(2, actual.Count);
            Assert.Equal(66, actual[0].Visits);
            Assert.Equal(33, actual[1].Visits);
        }

        [Fact]
        public void GetLatest_Should_return_most_popular_wikipages_limited()
        {
            WikiPage wiki1 = new WikiPage() { LastModified = new DateTime(2010, 1, 2) };
            WikiPage wiki2 = new WikiPage() { LastModified = new DateTime(2015, 1, 1) };
            WikiPage wiki3 = new WikiPage() { LastModified = new DateTime(2016, 1, 1) };
            WikiPage wiki4 = new WikiPage() { LastModified = new DateTime(2011, 1, 1) };

            collection.InsertOne(wiki1);
            collection.InsertOne(wiki2);
            collection.InsertOne(wiki3);
            collection.InsertOne(wiki4);

            var actual = wikiRepo.GetLatest(3);
            Assert.Equal(3, actual.Count);
            Assert.Equal(new DateTime(2016, 1, 1), actual[0].LastModified.ToLocalTime());
            Assert.Equal(new DateTime(2015, 1, 1), actual[1].LastModified.ToLocalTime());
            Assert.Equal(new DateTime(2011, 1, 1), actual[2].LastModified.ToLocalTime());
        }

        [Fact]
        public void Delete_should_remove_wikipage_from_database_by_path()
        {
            WikiPage wiki1 = new WikiPage() { Path = "SET" };
            collection.InsertOne(wiki1);

            Assert.Equal(1, collection.Find(_ => true).ToList().Count);

            wikiRepo.Delete(wiki1);

            Assert.Equal(0, collection.Find(_ => true).ToList().Count);
        }

        [Fact]
        public void Delete_should_not_remove_wrong_wikipage()
        {
            WikiPage wiki1 = new WikiPage() { Path = "GET" };
            collection.InsertOne(wiki1);

            Assert.Equal(1, collection.Find(_ => true).ToList().Count);

            wikiRepo.Delete(new WikiPage());

            Assert.Equal(1, collection.Find(_ => true).ToList().Count);
        }

        [Fact]
        public void Add_should_add_Wikipage_to_db()
        {
            WikiPage wiki = new WikiPage() { Path = "GET" };
            wikiRepo.Add(wiki);

            Assert.Equal(true, IsWikiPageEqual(wiki, collection.Find(_ => true).ToList()[0]));
        }

        [Fact]
        public void Update_Should_Update_WikiPage()
        {
            WikiPage wiki = new WikiPage() { Path = "Original", Title = "OK" };
            collection.InsertOne(wiki);
            wiki.Path = "Changed";
            wiki.Title = "BestTest";

            wikiRepo.Update(wiki);
            Assert.Equal("Changed", collection.Find(x => x.Id == wiki.Id).ToList()[0].Path);
            Assert.Equal("BestTest", collection.Find(x => x.Id == wiki.Id).ToList()[0].Title);
        }

        [Fact]
        public void AddVisit_Should_Update_Visits_In_WikiPage()
        {
            WikiPage wiki = new WikiPage() { Visits = 12, Path = "Original" };
            collection.InsertOne(wiki);
            wiki.Visits = 13;
            wiki.Path = "Changed";

            wikiRepo.AddVisit(wiki);
            Assert.Equal(13, collection.Find(x => x.Id == wiki.Id).ToList()[0].Visits);
            Assert.Equal("Original", collection.Find(x => x.Id == wiki.Id).ToList()[0].Path);
        }

        [Fact]
        public void RemoveFile_Should_Remove_Right_File()
        {
            WikiPage wiki = new WikiPage();
            
            Attachment file1 = new Attachment() { FileId = Guid.NewGuid() };
            Attachment file2 = new Attachment() { FileId = Guid.NewGuid() };
            wiki.Attachments = new List<Attachment>() { file1, file2 };

            collection.InsertOne(wiki);

            wikiRepo.RemoveFile(wiki, file1.FileId);
            Assert.Equal(1, collection.Find(x => x.Id == wiki.Id).ToList()[0].Attachments.Count);
            Assert.Equal(file2.FileId, collection.Find(x => x.Id == wiki.Id).ToList()[0].Attachments[0].FileId);
        }
    }
}
