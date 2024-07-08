using BoostLingo.Business;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Newtonsoft.Json.Linq;
using NUnit.Framework.Internal;


namespace BoostLingo.Tests
{
    
    public class DataManagerTests
    {

        private  IConfiguration Configuration;
        private Mock<ILogger<DataManager>> MockLogger;
        private DataManager DataManager;

        [SetUp]
        public void Setup()
        {
            var inMemorySettings =  new Dictionary<string, string> {{ "\"UrlSettings:Url\"", "http://api.com"}};

            Configuration = new ConfigurationBuilder()
                                .AddInMemoryCollection(inMemorySettings)
                                .Build();
           
            MockLogger = new Mock<ILogger<DataManager>>();
           

        }

        [Test]
        public async Task FetchDataAsync_ValidData_ReturnsListOfPersons()
        {
            // Arrange
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                                  .Setup<Task<HttpResponseMessage>>("SendAsync",
                                   ItExpr.IsAny<HttpRequestMessage>(),
                                   ItExpr.IsAny<CancellationToken>())
                                   .ReturnsAsync(new HttpResponseMessage
                                    {
                                        StatusCode = System.Net.HttpStatusCode.OK,
                                        Content = new StringContent("[{\"name\":\"John Doe\",\"language\":\"English\",\"id\":\"V59OF92YF627HFY0\",\"bio\":\"Test bio\",\"version\":1.0}]")
                                    });
         
            var httpClient = new HttpClient(mockHttpMessageHandler.Object);
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            mockHttpClientFactory.Setup(factory => factory.CreateClient(It.IsAny<string>()))
                                 .Returns(httpClient);
            DataManager = new DataManager(mockHttpClientFactory.Object, Configuration, MockLogger.Object);

            // Act
            var result = await DataManager.FetchDataAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Not.Empty);
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.Multiple(() =>
            {
                Assert.That(result[0].FirstName, Is.EqualTo("John"));
                Assert.That(result[0].LastName, Is.EqualTo("Doe"));
                Assert.That(result[0].UniqueId, Is.EqualTo("V59OF92YF627HFY0"));
                Assert.That(result[0].Language, Is.EqualTo("English"));
                Assert.That(result[0].PersonBio.BioText, Is.EqualTo("Test bio"));
                Assert.That(result[0].Version, Is.EqualTo(1.0));
            });

        }
    }
}
