using Blueprint.Controllers;
using Blueprint.Dtos;
using Blueprint.Services;
using Microsoft.AspNetCore.Mvc;
using NUnit;
using Moq;
using Blueprint.Repositories;
using Microsoft.AspNetCore.Http;

namespace BlueprintTest
{
    [TestFixture]
    public class ProductControllerTests
    {
        private Mock<IProductService> _mockProductService;
        private ProductController _controller;

        [SetUp]
        public void Setup()
        {
            _mockProductService = new Mock<IProductService>();
            _controller = new ProductController(_mockProductService.Object);

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["Authorization"] = "Bearer test-token";
            _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
        }

        [Test]
        public async Task GetProducts_ReturnsOk_WhenSuccess()
        {
            // Arrange
            var response = new AuthResult { Success = true, StatusCode = 200, Message = "Success", Data = new { } };
            _mockProductService.Setup(s => s.GetAllAsync(It.IsAny<string>())).ReturnsAsync(response);

            // Act
            var result = await _controller.GetProducts();

            // Assert
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.StatusCode, Is.EqualTo((int)ResponseCode.Success));
        }

        [Test]
        public async Task GetProducts_ReturnsNotFound_WhenStatusCodeIs404()
        {
            // Arrange
            var response = new AuthResult { Success = false, StatusCode = (int)ResponseCode.NotFound, Message = "Not Found", Data = null };
            _mockProductService.Setup(s => s.GetAllAsync(It.IsAny<string>())).ReturnsAsync(response);

            // Act
            var result = await _controller.GetProducts();

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.That(notFoundResult, Is.Not.Null);
            Assert.That(notFoundResult.StatusCode, Is.EqualTo((int)ResponseCode.NotFound));
        }

        [Test]
        public async Task GetProducts_ReturnsBadRequest_WhenStatusCodeIs400()
        {
            // Arrange
            var response = new AuthResult { Success = false, StatusCode = (int)ResponseCode.BadRequest, Message = "Bad Request", Data = null };
            _mockProductService.Setup(s => s.GetAllAsync(It.IsAny<string>())).ReturnsAsync(response);

            // Act
            var result = await _controller.GetProducts();

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);
            Assert.That(badRequestResult.StatusCode, Is.EqualTo((int)ResponseCode.BadRequest));
        }

        [Test]
        public async Task GetProducts_ReturnsInternalServerError_WhenUnexpectedError()
        {
            // Arrange
            var response = new AuthResult { Success = false, StatusCode = (int)ResponseCode.InternalServerError, Message = "Internal Server Error", Data = null };
            _mockProductService.Setup(s => s.GetAllAsync(It.IsAny<string>())).ReturnsAsync(response);

            // Act
            var result = await _controller.GetProducts();

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.That(statusCodeResult, Is.Not.Null);
            Assert.That(statusCodeResult.StatusCode, Is.EqualTo((int)ResponseCode.InternalServerError));
        }
    }
}