using ASPWiki.Controllers;
using ASPWiki.Model;
using ASPWiki.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using AutoMapper;
using ASPWiki.ViewModels;

namespace ASPWiki.Tests
{
    public class WikiControllerTest
    {
        Mock<IMapper> mapperMock;
        Mock<IRouteGenerator> mockRouteGen;
        Mock<IWikiService> mockWikiService;
        Mock<IWikiRepository> mockWikiRepo;
        Mock<IAuthorizationService> mockAuth;
        Mock<ILogger<WikiController>> mockLogger;

        public WikiControllerTest()
        {
            mapperMock = new Mock<IMapper>();
            mockRouteGen = new Mock<IRouteGenerator>();
            mockWikiService = new Mock<IWikiService>();
            mockWikiRepo = new Mock<IWikiRepository>();
            mockAuth = new Mock<IAuthorizationService>();
            mockLogger = new Mock<ILogger<WikiController>>();
        }

        [Fact]
        public void New_Should_Redirect_to_Add_Action()
        {
            string expectedTitle = "RedBanana";

            mockRouteGen.Setup(gen => gen.GenerateRoute()).Returns(expectedTitle);
            var controller = new WikiController(mapperMock.Object, mockRouteGen.Object, mockWikiRepo.Object, mockWikiService.Object, mockAuth.Object, mockLogger.Object);

            // Act
            var result = controller.New();

            // Assert
            RedirectToActionResult redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Add", redirectResult.ActionName);
            Assert.Equal(expectedTitle, redirectResult.RouteValues["title"]);
        }

        [Fact]
        public void Add_Should_Create_New_WikiPage_And_Load_Edit_View_With_Model()
        {
            var controller = new WikiController(mapperMock.Object, mockRouteGen.Object, mockWikiRepo.Object, mockWikiService.Object, mockAuth.Object, mockLogger.Object);

            // Act
            var result = controller.Add("ThisMapperHereNeedsMockingToCheckTitle");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Edit", viewResult.ViewName);
        }

        [Fact]
        public void Edit_Should_Redirect_To_Add_WikiPage_if_it_does_not_exists()
        {
            string expectedTitle = "Reall";
            string path = "Heh/This/Is/Not/" + expectedTitle;

            var controller = new WikiController(mapperMock.Object, mockRouteGen.Object, mockWikiRepo.Object, mockWikiService.Object, new AuthorizeStub(true), mockLogger.Object);

            // Act
            var result = controller.Edit(path);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result.Result);

            Assert.Equal("Add", redirectResult.ActionName);
            Assert.Equal(expectedTitle, redirectResult.RouteValues["title"]);
        }

        [Fact]
        public void Edit_Should_Edit_Existing_WikiPage()
        {
            string expectedTitle = "BlueBanana";
            string path = "Heh/This/Is/" + expectedTitle;

            WikiPage wikiPage = new WikiPage(expectedTitle);

            WikipageEdit wikiPageEdit= new WikipageEdit();
            wikiPageEdit.Title = expectedTitle;

            mockWikiRepo.Setup(repo => repo.GetByPath(path)).Returns(wikiPage);
            mapperMock.Setup(m => m.Map<WikipageEdit>(wikiPage)).Returns(wikiPageEdit);
            var controller = new WikiController(mapperMock.Object, mockRouteGen.Object, mockWikiRepo.Object, mockWikiService.Object, new AuthorizeStub(true), mockLogger.Object);

            // Act
            var result = controller.Edit(path);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result.Result);

            var model = Assert.IsAssignableFrom<WikipageEdit>(viewResult.ViewData.Model);
            Assert.Equal(expectedTitle, model.Title);
            Assert.Equal("Edit", viewResult.ViewName);
        }

        [Fact]
        public void Edit_Should_return_Challenge_result_if_not_authenticated()
        {
            string expectedTitle = "BlueBanana";
            string path = "Heh/This/Is/" + expectedTitle;

            WikiPage wikiPage = new WikiPage(expectedTitle);

            mockWikiRepo.Setup(repo => repo.GetByPath(path)).Returns(wikiPage);
            var controller = new WikiController(mapperMock.Object, mockRouteGen.Object, mockWikiRepo.Object, mockWikiService.Object, new AuthorizeStub(false), mockLogger.Object);

            // Act
            var result = controller.Edit(path);

            // Assert
            ChallengeResult redirectResult = Assert.IsType<ChallengeResult>(result.Result);
        }

        [Fact]
        public void View_Should_Redirect_To_Not_Found_If_WikiPage_does_not_exists()
        {
            string wikiPageRoute = "Omg/This/Exists/Not";

            mapperMock.Setup(m => m.Map<WikipageView>(new WikiPage())).Returns(new WikipageView());
            var controller = new WikiController(mapperMock.Object, mockRouteGen.Object, mockWikiRepo.Object, mockWikiService.Object, new AuthorizeStub(true), mockLogger.Object);

            // Act
            var result = controller.ViewPage(wikiPageRoute, null);

            // Assert
            RedirectToActionResult redirectResult = Assert.IsType<RedirectToActionResult>(result.Result);
            Assert.Equal("NotFound", redirectResult.ActionName);
            Assert.Equal("Not", redirectResult.RouteValues["title"]);
        }

        [Fact]
        public void View_Should_Show_WikiPage()
        {
            string wikiPageRoute = "Omg/This/Exists";

            WikiPage wikiPage = new WikiPage();

            WikipageView wikiPageView = new WikipageView();
            wikiPageView.Title = "Exists";
            wikiPageView.ContentHistory = new List<string>(new string[] { "v0", "v1", "current" });
            wikiPageView.Content = "current";

            mockWikiRepo.Setup(repo => repo.GetByPath(wikiPageRoute)).Returns(wikiPage);
            mapperMock.Setup(m => m.Map<WikipageView>(wikiPage)).Returns(wikiPageView);
            var controller = new WikiController(mapperMock.Object, mockRouteGen.Object, mockWikiRepo.Object, mockWikiService.Object, new AuthorizeStub(true), mockLogger.Object);

            // Act
            var result = controller.ViewPage(wikiPageRoute, null);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result.Result);

            var model = Assert.IsAssignableFrom<WikipageView>(viewResult.ViewData.Model);
            Assert.Equal(wikiPageView.Title, model.Title);
            Assert.Equal(wikiPageView.Content, model.Content);
            Assert.Equal(wikiPageView.ContentHistory.Count, model.ContentHistory.Count);
            Assert.Equal("View", viewResult.ViewName);
        }

        [Fact]
        public void View_should_return_ChallengeResult_if_not_authenticated()
        {
            string wikiPageRoute = "Omg/This/Exists";

            WikiPage wikiPage = new WikiPage("Exists");
            wikiPage.ContentHistory = new List<string>(new string[] { "v0", "v1", "current" });

            mockWikiRepo.Setup(repo => repo.GetByPath(wikiPageRoute)).Returns(wikiPage);
            var controller = new WikiController(mapperMock.Object, mockRouteGen.Object, mockWikiRepo.Object, mockWikiService.Object, new AuthorizeStub(false), mockLogger.Object);

            // Act
            var result = controller.ViewPage(wikiPageRoute, "1");

            // Assert
            ChallengeResult redirectResult = Assert.IsType<ChallengeResult>(result.Result);
        }

        [Fact]
        public void Delete_should_redirect_to_main()
        {
            string pathToDelete = "Delete/This/Page";
            
            var controller = new WikiController(mapperMock.Object, mockRouteGen.Object, mockWikiRepo.Object, mockWikiService.Object, new AuthorizeStub(true), mockLogger.Object);
            controller.TempData = new TempDataDictionary(new Mock<HttpContext>().Object, new Mock<ITempDataProvider>().Object);

            // Act
            var result = controller.Delete(pathToDelete);

            // Assert
            RedirectToActionResult redirectResult = Assert.IsType<RedirectToActionResult>(result.Result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public void Delete_should_return_ChallengeResult_if_not_authenticated()
        {
            string pathToDelete = "Delete/This/Page";

            var controller = new WikiController(mapperMock.Object, mockRouteGen.Object, mockWikiRepo.Object, mockWikiService.Object, new AuthorizeStub(false), mockLogger.Object);
            controller.TempData = new TempDataDictionary(new Mock<HttpContext>().Object, new Mock<ITempDataProvider>().Object);

            // Act
            var result = controller.Delete(pathToDelete);

            // Assert
            ChallengeResult redirectResult = Assert.IsType<ChallengeResult>(result.Result);
        }
    }
}
