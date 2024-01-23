using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using Moq.Protected;
using Xunit;

namespace PixelService.Tests
{
    public class PixelServiceTests
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly Mock<HttpMessageHandler> _mockMessageHandler;

        public PixelServiceTests()
        {
            _factory = new WebApplicationFactory<Program>();
            _mockMessageHandler = new Mock<HttpMessageHandler>();
        }

        [Fact]
        public async Task TrackEndpoint_ReturnsTransparentGif()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/track");

            Assert.Equal("image/gif", response.Content.Headers.ContentType.MediaType);
            var gifBytes = await response.Content.ReadAsByteArrayAsync();
            var expectedGif = Convert.FromBase64String("R0lGODlhAQABAIAAAP///wAAACH5BAEAAAAALAAAAAABAAEAAAICRAEAOw==");
            Assert.Equal(expectedGif, gifBytes);
        }

        [Fact]
        public async Task TrackEndpoint_SendsDataToStorageService()
        {
            // Setup the mock handler
            _mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("", Encoding.UTF8, "application/json")
                });

            // Create an instance of HttpClient with HttpMessageHandlerMock
            var client = new HttpClient(new HttpMessageHandlerMock(_mockMessageHandler))
            {
                BaseAddress = new Uri("http://localhost:5167")
            };

            // Your existing test logic
            var response = await client.GetAsync("/track");

            // Assertions
            _mockMessageHandler.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post &&
                    req.RequestUri.ToString().Contains("http://localhost:5016/store")),
                ItExpr.IsAny<CancellationToken>()
            );
        }
    }

    internal class HttpMessageHandlerMock : DelegatingHandler
    {
        private readonly Mock<HttpMessageHandler> _mockHandler;

        public HttpMessageHandlerMock(Mock<HttpMessageHandler> mockHandler)
        {
            _mockHandler = mockHandler;
            InnerHandler = _mockHandler.Object;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return base.SendAsync(request, cancellationToken);
        }
    }

}
