// using Coders_Back.Domain.Interfaces;
// using Coders_Back.Host.Controllers;
// using FluentAssertions;
// using Microsoft.AspNetCore.Mvc;
// using Moq;
// using Xunit;
//
// namespace Coders_Back.UnitTest.Host.Controllers;
//
// public class RequestControllerTests : UnitTestBaseUtils
// {
//     private readonly Mock<IRequestService> _requestServiceMock;
//     private readonly RequestController _controller;
//
//     public RequestControllerTests()
//     {
//         _requestServiceMock = new Mock<IRequestService>();
//         _controller = new RequestController(_requestServiceMock.Object);
//     }
//
//     [Fact(DisplayName = "Accept request with valid id should return OK result")]
//     public async Task Accept_WithValidId_ShouldReturnOkResult()
//     {
//         // Arrange
//         var requestId = Guid.NewGuid();
//         var userId = Guid.NewGuid();
//         var result = true;
//
//         _requestServiceMock.Setup(s => s.Accept(requestId, userId)).ReturnsAsync(result);
//
//         // Act
//         var response = await _controller.Accept(requestId);
//
//         // Assert
//         response.Should().BeOfType<OkResult>();
//     }
//
//     [Fact(DisplayName = "Accept request with invalid id should return BadRequest result")]
//     public async Task Accept_WithInvalidId_ShouldReturnBadRequestResult()
//     {
//         // Arrange
//         var requestId = Guid.NewGuid();
//         var userId = Guid.NewGuid();
//         var result = false;
//
//         _requestServiceMock.Setup(s => s.Accept(requestId, userId)).ReturnsAsync(result);
//
//         // Act
//         var response = await _controller.Accept(requestId);
//
//         // Assert
//         response.Should().BeOfType<BadRequestResult>();
//     }
//
//     [Fact(DisplayName = "Reject request with valid id should return OK result")]
//     public async Task Reject_WithValidId_ShouldReturnOkResult()
//     {
//         // Arrange
//         var requestId = Guid.NewGuid();
//         var userId = Guid.NewGuid();
//         var result = true;
//
//         _requestServiceMock.Setup(s => s.Reject(requestId, userId)).ReturnsAsync(result);
//
//         // Act
//         var response = await _controller.Reject(requestId);
//
//         // Assert
//         response.Should().BeOfType<OkResult>();
//     }
//
//     [Fact(DisplayName = "Reject request with invalid id should return BadRequest result")]
//     public async Task Reject_WithInvalidId_ShouldReturnBadRequestResult()
//     {
//         // Arrange
//         var requestId = Guid.NewGuid();
//         var userId = Guid.NewGuid();
//         var result = false;
//
//         _requestServiceMock.Setup(s => s.Reject(requestId, userId)).ReturnsAsync(result);
//
//         // Act
//         var response = await _controller.Reject(requestId);
//
//         // Assert
//         response.Should().BeOfType<BadRequestResult>();
//     }
// }