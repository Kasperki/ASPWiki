
using ASPWiki.Controllers;
using ASPWiki.Model;
using ASPWiki.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace ASPWiki.Tests
{
    public class HomeControllerTest
    {
        private HomeController controller;
        private Mock<IMapper> mockMapper;
        private Mock<IWikiService> mockWikiService;
        private Mock<IWikiRepository> mockWikiRepo;

        public HomeControllerTest()
        {
            // Arrange
            mockMapper = new Mock<IMapper>();
            mockWikiService = new Mock<IWikiService>();
            mockWikiRepo = new Mock<IWikiRepository>();

            controller = new HomeController(mockMapper.Object, mockWikiRepo.Object, mockWikiService.Object);
        }

        [Fact]
        public void Error_Page_Should_Show_NotFound_If_code_is_404()
        {
            // Act
            var result = controller.Error(404);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("PageNotFound", viewResult.ViewName);
        }

        [Fact]
        public void Error_Page_should_Show_error_view()
        {
            // Act
            var result = controller.Error(200);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public void Forbidden_Page_should_Show_Forbidden_view()
        {
            // Act
            var result = controller.Forbidden();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Forbidden", viewResult.ViewName);
        }

        [Fact]
        public void GetWikiTree_should_return_JsonResult_with_right_data()
        {
            List<Node> nodes = new List<Node>()
            {
                new Node("a") { },
                new Node("b") { },
            };

            mockWikiRepo.Setup(repo => repo.GetAll()).Returns(new List<WikiPage>());
            mockWikiService.Setup(repo => repo.GetWikiTree(It.IsAny<List<WikiPage>>())).Returns(nodes);
            var controller = new HomeController(mockMapper.Object, mockWikiRepo.Object, mockWikiService.Object);

            // Act
            var result = controller.GetAsideWikiPages();

            // Assert
            JsonResult jsonResult = Assert.IsType<JsonResult>(result);
            Assert.IsType<List<Node>>(jsonResult.Value);
        }
    }
}
