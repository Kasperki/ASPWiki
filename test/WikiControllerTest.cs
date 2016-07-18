using ASPWiki.Controllers;
using ASPWiki.Model;
using ASPWiki.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

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
        public void View_Should_Redirect_To_Not_Found_If_WikiPage_does_not_exists()
        {
            string wikiPageRoute = "Omg/This/Exists/Not";

            // Arrange
            var mockRouteGen = new Mock<IRouteGenerator>();
            var mockWikiService = new Mock<IWikiService>();
            var mockWikiRepo = new Mock<IWikiRepository>();

            var controller = new WikiController(mockRouteGen.Object, mockWikiRepo.Object, mockWikiService.Object);

            // Act
            var result = controller.View(wikiPageRoute);

            // Assert
            RedirectToActionResult redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("NotFound", redirectResult.ActionName);
            Assert.Equal("Not", redirectResult.RouteValues["title"]);
        }

        [Fact]
        public void View_Should_Show_WikiPage()
        {
            string wikiPageRoute = "Omg/This/Exists";
            string[] path = new string[] { "Omg", "This", "Exists" };

            // Arrange
            var mockRouteGen = new Mock<IRouteGenerator>();
            var mockWikiService = new Mock<IWikiService>();
            var mockWikiRepo = new Mock<IWikiRepository>();

            mockWikiRepo.Setup(repo => repo.GetByPath(path)).Returns(new WikiPage("Exists"));
            var controller = new WikiController(mockRouteGen.Object, mockWikiRepo.Object, mockWikiService.Object);

            // Act
            var result = controller.View(wikiPageRoute);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);

            var model = Assert.IsAssignableFrom<WikiPage>(viewResult.ViewData.Model);
            Assert.Equal("Exists", model.Title);
            Assert.Equal("View", viewResult.ViewName);
        }

    }
}
