using ASPWiki.Controllers;
using ASPWiki.Model;
using ASPWiki.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Http;

namespace ASPWiki.Tests
{
    public class WikiControllerTest
    {
        [Fact]
        public void New_Should_Redirect_to_Add_Action()
        {
            string expectedTitle = "RedBanana";
            
            // Arrange
            var mockRouteGen = new Mock<IRouteGenerator>();
            var mockWikiService = new Mock<IWikiService>();
            var mockWikiRepo = new Mock<IWikiRepository>();

            mockRouteGen.Setup(gen => gen.GenerateRoute()).Returns(expectedTitle);

            var controller = new WikiController(mockRouteGen.Object, mockWikiRepo.Object, mockWikiService.Object);

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
            string expectedTitle = "BlueBanana";

            // Arrange
            var mockRouteGen = new Mock<IRouteGenerator>();
            var mockWikiService = new Mock<IWikiService>();
            var mockWikiRepo = new Mock<IWikiRepository>();

            var controller = new WikiController(mockRouteGen.Object, mockWikiRepo.Object, mockWikiService.Object);

            // Act
            var result = controller.Add(expectedTitle);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            
            var model = Assert.IsAssignableFrom<WikiPage>(viewResult.ViewData.Model);
            Assert.Equal(expectedTitle, model.Title);
            Assert.Equal("Edit", viewResult.ViewName);
        }

        [Fact]
        public void Edit_Should_Redirect_To_Add_WikiPage_if_it_does_not_exists()
        {
            string expectedTitle = "Reall";
            string path = "Heh/This/Is/Not/" + expectedTitle;

            // Arrange
            var mockRouteGen = new Mock<IRouteGenerator>();
            var mockWikiService = new Mock<IWikiService>();
            var mockWikiRepo = new Mock<IWikiRepository>();

            var controller = new WikiController(mockRouteGen.Object, mockWikiRepo.Object, mockWikiService.Object);

            // Act
            var result = controller.Edit(path);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal("Add", redirectResult.ActionName);
            Assert.Equal(expectedTitle, redirectResult.RouteValues["title"]);
        }

        [Fact]
        public void Edit_Should_Edit_Existing_WikiPage()
        {
            string expectedTitle = "BlueBanana";
            string path = "Heh/This/Is/" + expectedTitle;
            string[] pathArray = path.Split('/');

            // Arrange
            var mockRouteGen = new Mock<IRouteGenerator>();
            var mockWikiService = new Mock<IWikiService>();
            var mockWikiRepo = new Mock<IWikiRepository>();

            WikiPage wikiPage = new WikiPage(expectedTitle);
            wikiPage.Path = path;

            mockWikiRepo.Setup(repo => repo.GetByPath(path)).Returns(wikiPage);
            var controller = new WikiController(mockRouteGen.Object, mockWikiRepo.Object, mockWikiService.Object);

            // Act
            var result = controller.Edit(path);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);

            var model = Assert.IsAssignableFrom<WikiPage>(viewResult.ViewData.Model);
            Assert.Equal(expectedTitle, model.Title);
            Assert.Equal("Edit", viewResult.ViewName);
        }

        [Fact]
        public void View_Should_Redirect_To_Not_Found_If_WikiPage_does_not_exists()
        {
            string wikiPageRoute = "Omg/This/Exists/Not";

            // Arrange
            var mockRouteGen = new Mock<IRouteGenerator>();
            var mockWikiService = new Mock<IWikiService>();
            var mockWikiRepo = new Mock<IWikiRepository>();

            var controller = new WikiController(mockRouteGen.Object, mockWikiRepo.Object, mockWikiService.Object);

            // Act
            var result = controller.ViewPage(wikiPageRoute, null);

            // Assert
            RedirectToActionResult redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("NotFound", redirectResult.ActionName);
            Assert.Equal("Not", redirectResult.RouteValues["title"]);
        }

        [Fact]
        public void View_Should_Show_WikiPage()
        {
            string wikiPageRoute = "Omg/This/Exists";

            // Arrange
            var mockRouteGen = new Mock<IRouteGenerator>();
            var mockWikiService = new Mock<IWikiService>();
            var mockWikiRepo = new Mock<IWikiRepository>();

            WikiPage wikiPage = new WikiPage("Exists");
            wikiPage.ContentHistory = new List<string>(new string[] { "v0", "v1", "current" });
            wikiPage.Content = "current";

            mockWikiRepo.Setup(repo => repo.GetByPath(wikiPageRoute)).Returns(wikiPage);
            var controller = new WikiController(mockRouteGen.Object, mockWikiRepo.Object, mockWikiService.Object);

            // Act
            var result = controller.ViewPage(wikiPageRoute, null);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);

            var model = Assert.IsAssignableFrom<WikiPage>(viewResult.ViewData.Model);
            Assert.Equal("Exists", model.Title);
            Assert.Equal(wikiPage.ContentHistory[wikiPage.ContentHistory.Count - 1], model.Content);
            Assert.Equal("View", viewResult.ViewName);
        }

        [Fact]
        public void View_Should_Show_WikiPage_With_Right_Version()
        {
            string wikiPageRoute = "Omg/This/Exists";

            // Arrange
            var mockRouteGen = new Mock<IRouteGenerator>();
            var mockWikiService = new Mock<IWikiService>();
            var mockWikiRepo = new Mock<IWikiRepository>();

            WikiPage wikiPage = new WikiPage("Exists");
            wikiPage.ContentHistory = new List<string>(new string[] { "v0", "v1", "current" });

            mockWikiRepo.Setup(repo => repo.GetByPath(wikiPageRoute)).Returns(wikiPage);
            var controller = new WikiController(mockRouteGen.Object, mockWikiRepo.Object, mockWikiService.Object);

            // Act
            var result = controller.ViewPage(wikiPageRoute, "1");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);

            var model = Assert.IsAssignableFrom<WikiPage>(viewResult.ViewData.Model);
            Assert.Equal(wikiPage.ContentHistory[1], model.Content);
            Assert.Equal("View", viewResult.ViewName);
        }

        [Fact]
        public void Delete_should_redirect_to_main()
        {
            string pathToDelete = "Delete/This/Page";
            
            // Arrange
            var mockRouteGen = new Mock<IRouteGenerator>();
            var mockWikiService = new Mock<IWikiService>();
            var mockWikiRepo = new Mock<IWikiRepository>();


            var controller = new WikiController(mockRouteGen.Object, mockWikiRepo.Object, mockWikiService.Object);
            controller.TempData = new TempDataDictionary(new Mock<HttpContext>().Object, new Mock<ITempDataProvider>().Object);

            // Act
            var result = controller.Delete(pathToDelete);

            // Assert
            RedirectToActionResult redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

    }
}
