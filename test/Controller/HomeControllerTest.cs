
using ASPWiki.Controllers;
using ASPWiki.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace ASPWiki.Tests
{
    public class HomeControllerTest
    {
        private HomeController controller;

        public HomeControllerTest()
        {
            // Arrange
            var mockMapper = new Mock<IMapper>();
            var mockWikiService = new Mock<IWikiService>();
            var mockWikiRepo = new Mock<IWikiRepository>();

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

    }
}
