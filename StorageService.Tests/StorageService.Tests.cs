using System.IO.Abstractions.TestingHelpers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Text;
using Xunit;

namespace StorageService.Tests
{
    public class StorageServiceTests
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly MockFileSystem _mockFileSystem;

        public StorageServiceTests()
        {
            _factory = new WebApplicationFactory<Program>();
            _mockFileSystem = new MockFileSystem();
        }

        [Fact]
        public async Task StoreEndpoint_WritesToFile()
        {
            var client = _factory.CreateClient();
            var content = new StringContent("2022-12-19T14:16:49.9605280Z|null|null|192.168.1.1", Encoding.UTF8, "application/x-www-form-urlencoded");

            await client.PostAsync("/store", content);

            var logFilePath = "F:\\tmp\\visits.log";
            Assert.True(_mockFileSystem.File.Exists(logFilePath));
            var fileContent = _mockFileSystem.File.ReadAllText(logFilePath);
            Assert.Contains("2022-12-19T14:16:49.9605280Z|null|null|192.168.1.1", fileContent);
        }
    }
}
