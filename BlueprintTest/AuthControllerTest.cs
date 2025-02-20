using Blueprint.Controllers;
using Blueprint.Dtos;
using Blueprint.Services;
using Microsoft.AspNetCore.Mvc;
using NUnit;
using Moq;

namespace BlueprintTest
{
    [TestFixture]
    public class AuthControllerTest
    {
        private Mock<IAuthService> _authServiceMock;
        private AuthController _controller;

        [SetUp]
        public void Setup()
        {
            _authServiceMock = new Mock<IAuthService>();
            _controller = new AuthController(_authServiceMock.Object);
        }

        [Test]
        public async Task Register_ValidUser_ReturnsOk()
        {
            // Arrange
            var registerDto = new RegisterDto { Username = "testuser", Password = "password123" };
            var authResult = new AuthResult { Success = true };

            _authServiceMock.Setup(s => s.Register(registerDto))
                .ReturnsAsync(authResult);

            // Act
            var result = await _controller.Register(registerDto) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo((int)ResponseCode.Success));
        }

        [Test]
        public async Task Register_DuplicateUser_ReturnsConflict()
        {
            // Arrange
            var registerDto = new RegisterDto { Username = "testuser", Password = "password123" };
            var authResult = new AuthResult { Success = false, StatusCode = (int)ResponseCode.Conflict, Message = "Username already taken." };

            _authServiceMock.Setup(s => s.Register(registerDto))
                .ReturnsAsync(authResult);

            // Act
            var result = await _controller.Register(registerDto) as ConflictObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo((int)ResponseCode.Conflict));
        }

        [Test]
        public async Task Login_ValidUser_ReturnsOk()
        {
            // Arrange
            var loginDto = new LoginDto { Username = "testuser", Password = "password123" };
            var authResult = new AuthResult { Success = true };

            _authServiceMock.Setup(s => s.Login(loginDto))
                .ReturnsAsync(authResult);

            // Act
            var result = await _controller.Login(loginDto) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo((int)ResponseCode.Success));
        }

        [Test]
        public async Task Login_User_ReturnsBadRequest()
        {
            // Arrange
            var loginDto = new LoginDto { Username = "testuser", Password = "password123" };
            var authResult = new AuthResult { Success = false, StatusCode = (int)ResponseCode.BadRequest, Message = "Invalid credentials." };

            _authServiceMock.Setup(s => s.Login(loginDto))
                .ReturnsAsync(authResult);

            // Act
            var result = await _controller.Login(loginDto) as BadRequestObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo((int)ResponseCode.BadRequest));
        }
    }
}