using System;
using ASPWiki.Controllers;
using Xunit;
using Moq;
using ASPWiki.Services;
using Microsoft.AspNetCore.Mvc;
using ASPWiki.Model;
using System.Collections.Generic;

namespace ASPWiki.Tests
{
    public class FileControllerTest
    {
        [Fact]
        public void Get_File__Should_Return_ForbidResult_If_Not_Authorized()
        {
            // Arrange
            var mockWikiRepo = new Mock<IWikiRepository>();
            var mockAuth = new AuthorizeStub(false);

            var controller = new FileController(mockWikiRepo.Object, mockAuth);

            // Act
            var result = controller.GetFile(Guid.NewGuid(), Guid.NewGuid());

            // Assert
            ForbidResult forbidResult = Assert.IsType<ForbidResult>(result.Result);
        }

        [Fact]
        public void Get_File_Should_Return_Bad_Request_If_Attachment_Is_Not_OK()
        {
            WikiPage wikiPage = new WikiPage();
            wikiPage.Id = Guid.NewGuid();

            Attachment attacment = new Attachment();
            attacment.FileId = Guid.NewGuid();
            wikiPage.Attachments = new List<Attachment>();
            wikiPage.Attachments.Add(attacment);

            // Arrange
            var mockWikiRepo = new Mock<IWikiRepository>();
            var mockAuth = new AuthorizeStub(true);

            mockWikiRepo.Setup(x => x.GetById(wikiPage.Id)).Returns(wikiPage);

            var controller = new FileController(mockWikiRepo.Object, mockAuth);

            // Act
            var result = controller.GetFile(wikiPage.Id, attacment.FileId);

            // Assert
            BadRequestResult badReqest = Assert.IsType<BadRequestResult>(result.Result);
        }

        [Fact]
        public void Get_File__Should_Return_Attacment_If_Authorized()
        {
            string expectedName = "ThisFile";

            WikiPage wikiPage = new WikiPage();
            wikiPage.Id = Guid.NewGuid();

            Attachment attacment = new Attachment();
            attacment.FileId = Guid.NewGuid();
            attacment.FileName = expectedName;
            attacment.ContentType = "text/plain";
            attacment.Content = new byte[100];

            wikiPage.Attachments = new List<Attachment>();
            wikiPage.Attachments.Add(attacment);

            // Arrange
            var mockWikiRepo = new Mock<IWikiRepository>();
            var mockAuth = new AuthorizeStub(true);

            mockWikiRepo.Setup(x => x.GetById(wikiPage.Id)).Returns(wikiPage);

            var controller = new FileController(mockWikiRepo.Object, mockAuth);

            // Act
            var result = controller.GetFile(wikiPage.Id, attacment.FileId);

            // Assert
            FileContentResult fileContentResult = Assert.IsType<FileContentResult>(result.Result);
            Assert.Equal(expectedName, fileContentResult.FileDownloadName);
        }


        [Fact(Skip = "NotReady")]
        public void DeleteFile__Should_Return_Bad_Request_If_Not_Valid_Guid()
        {
            // Arrange
            var mockWikiRepo = new Mock<IWikiRepository>();
            var mockAuth = new AuthorizeStub(true);

            var controller = new FileController(mockWikiRepo.Object, mockAuth);

            // Act
            var result = controller.DeleteFile("kek");
            
            // Assert
            BadRequestResult badReqest = Assert.IsType<BadRequestResult>(result);
        }
    }
}
