using Xunit;
using ASPWiki.Services;
using ASPWiki.Model;
using Moq;
using System;
using System.Collections.Generic;

namespace ASPWiki.Tests
{
    public class WikiServiceTest
    {
        private readonly WikiService wikiService;

        private WikiPage w1, w2, w22;
        private string[] pathW1, pathW2, pathW22;

        public WikiServiceTest()
        {
            w1 = new WikiPage("w1");
            w1.SetPath(new List<string> { "w1" }); //W1 PATH: W1

            w2 = new WikiPage("w2");
            w2.SetPath(w1.Path); //W2 PATH: W1 > W2

            w22 = new WikiPage("w2");
            w22.SetPath(w2.Path); //W22 PATH: W1 > W2 > W2

            Mock<IWikiRepository> wikiRepoMock = new Mock<IWikiRepository>();

            pathW1 = new string[] { "w1" };
            pathW2 = new string[] { "w1", "w2" };
            pathW22 = new string[] { "w1", "w2", "w2" };

            wikiRepoMock.Setup(repo => repo.GetByPath(pathW1)).Returns(w1);
            wikiRepoMock.Setup(repo => repo.GetByPath(pathW2)).Returns(w2);
            wikiRepoMock.Setup(repo => repo.GetByPath(pathW22)).Returns(w22);

            IWikiRepository wikiRepo = wikiRepoMock.Object;
            wikiService = new WikiService(wikiRepo);
        }

        [Fact]
        public void ValidatePath() 
        {       
        //PATH VALID

            //TEST SAVING TO SAME PAHT IS VALID.
            Assert.True(wikiService.IsValidPath(pathW1, w1.Id));    //W1  PATH: W1
            Assert.True(wikiService.IsValidPath(pathW2, w2.Id));    //W2  PATH: W1 > W2
            Assert.True(wikiService.IsValidPath(pathW22, w22.Id));  //W22 PATH: W1 > W2 > W2

            //TEST THAT SAVING TO OPEN PATH IS VALID
            Assert.True(wikiService.IsValidPath(new string[] { "w2" }, w22.Id)); //W22 PATH: W2 
            Assert.True(wikiService.IsValidPath(new string[] { "w3" }, w22.Id)); //W22 PATH: W3 
            Assert.True(wikiService.IsValidPath(new string[] { "w1" , "w4" }, w22.Id)); //W22 PATH: W1 > W4 
            Assert.True(wikiService.IsValidPath(new string[] { "w1", "w2", "w4" }, w22.Id)); //W22 PATH: W1 > W2 > W4

        //PATH NOT VALID

            //TEST THAT DUPLICATE PATHS ARE INVALID
            Exception e = Assert.Throws<Exception>(() => wikiService.IsValidPath(pathW1, w22.Id)); //W22 PATH: W1
            Assert.Equal("Path already exists", e.Message);

            Exception e2 = Assert.Throws<Exception>(() => wikiService.IsValidPath(pathW2, w22.Id)); //W22 PATH: W1 > W2
            Assert.Equal("Path already exists", e2.Message);

            //TEST THAT IF PATH DOES NOT EXIST THROW ERROR
            Exception e3 = Assert.Throws<Exception>(() => wikiService.IsValidPath(new string[] { "w3", "w1" }, w1.Id)); //W1 PATH: W3 > W1
            Assert.Equal("Parent not found", e3.Message);

            Exception e4 = Assert.Throws<Exception>(() => wikiService.IsValidPath(new string[] { "w1", "w6", "w1" }, w1.Id)); //W1 PATH: W1 > W6 > W1
            Assert.Equal("Parent not found", e4.Message);
        }
    }
}
