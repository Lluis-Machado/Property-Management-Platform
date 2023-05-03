using Authentication.Controllers;
using Authentication.Utils;
using AuthenticationAPI.Services.Auth0.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Net;

namespace AuthenticationUnitTests
{
    public class TokenControllerTests
    {
        private readonly Mock<IPublicTokenAPI> _mockPublicTokenApi;
        private readonly TokenController _tokenController;

        public TokenControllerTests()
        {
            _mockPublicTokenApi = new Mock<IPublicTokenAPI>();
            _tokenController = new TokenController(_mockPublicTokenApi.Object);
        }

        [Fact]
        public async Task GetToken_ReturnsOk_WhenValidCredentialsProvided()
        {
            // Arrange
            string username = "lisandro@itcplatform.de";
            string password = "Test1234!";
            var expectedResponse = new
            {
                access_token = "validToken",
                expires_in = 86400,
                token_type = "Bearer"
            };
            _mockPublicTokenApi.Setup(api => api.GetTokenAsync(username, password)).ReturnsAsync(expectedResponse);

            // Act
            var result = await _tokenController.GetToken(username, password);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            dynamic response = okResult!.Value!;

            Assert.NotNull(response.access_token);
            Assert.True(response.access_token.ToString().Length > 0);
            Assert.Equal(expectedResponse.expires_in, response.expires_in);
            Assert.Equal(expectedResponse.token_type, response.token_type);
        }

        [Fact]
        public async Task GetToken_ReturnsForbidden_WhenInvalidCredentialsProvided()
        {
            // Arrange
            string username = "invalidUser";
            string password = "invalidPassword";
            _mockPublicTokenApi.Setup(api => api.GetTokenAsync(username, password)).ThrowsAsync(new ApiException(HttpStatusCode.Forbidden, ""));

            // Act
            async Task act() => await _tokenController.GetToken(username, password);

            // Assert
            ApiException ae = await Assert.ThrowsAsync<ApiException>(act);
            Assert.Equal(HttpStatusCode.Forbidden, ae.StatusCode);
        }

        [Fact]
        public async Task GetToken_ReturnsTooManyRequests_WhenRateLimitExceeded()
        {
            // Arrange
            string username = "testuser";
            string password = "testpassword";
            _mockPublicTokenApi.Setup(api => api.GetTokenAsync(username, password)).ThrowsAsync(new ApiException(HttpStatusCode.TooManyRequests, ""));

            // Act
            async Task act() => await _tokenController.GetToken(username, password);

            // Assert
            ApiException ae = await Assert.ThrowsAsync<ApiException>(act);
            Assert.Equal(HttpStatusCode.TooManyRequests, ae.StatusCode);
        }

        [Fact]
        public async Task GetToken_ReturnsInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            string username = "testuser";
            string password = "testpassword";
            _mockPublicTokenApi.Setup(api => api.GetTokenAsync(username, password)).ThrowsAsync(new Exception("Internal server error"));

            // Act
            async Task act() => await _tokenController.GetToken(username, password);

            // Assert
            await Assert.ThrowsAsync<Exception>(act);
        }
    }
}