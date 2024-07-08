using BoostLingo.Core;
using BoostLingo.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoostLingo.Tests
{
    public class PersonRepositoryTests
    {

        private PersonContext PersonContext;
        private IConfiguration Configuration;
        private Mock<ILogger<PersonRepository>> MockLogger;
        private PersonRepository PersonRepository;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<PersonContext>()
                .UseInMemoryDatabase(databaseName: "PersonTestDb")
                .Options;

            PersonContext = new PersonContext(options);
            MockLogger = new Mock<ILogger<PersonRepository>>();

            var inMemorySettings = new Dictionary<string, string> { { "BatchSize", "10" }, { "MaxRetryCount", "3" } };

            Configuration = new ConfigurationBuilder()
                                .AddInMemoryCollection(inMemorySettings)
                                .Build();

            PersonRepository = new PersonRepository(PersonContext, Configuration, MockLogger.Object);
        }

        [TearDown]
        public void TearDown()
        {
            PersonContext.Dispose();
        }

        [Test]
        public async Task AddPersonsAsync_ValidData_AddsPersonsToDatabase()
        {
            // Arrange
            var persons = new List<Person>
        {
            new Person
            {
                UniqueId = "5ZVOEPMJUI4MB4EN",
                FirstName = "John",
                LastName = "Doe",
                Language = "English",
                Version = 1.0,
                PersonBio = new PersonBio { BioText = "Test bio" }
            }
        };

            // Act
            await PersonRepository.AddPersonsAsync(persons);

            // Assert
            var addedPersons = PersonContext.Persons.Include(p => p.PersonBio).ToList();
            Assert.That(addedPersons.Count, Is.EqualTo(1));
            Assert.That(addedPersons[0].FirstName, Is.EqualTo("John"));
            Assert.That(addedPersons[0].LastName, Is.EqualTo("Doe"));
            Assert.IsNotNull(addedPersons[0].PersonBio);
            Assert.That(addedPersons[0].PersonBio.BioText, Is.EqualTo("Test bio"));
        }

        [Test]
        public async Task GetSortedPersonsAsync_ReturnsSortedPersons()
        {
            // Arrange
            var persons = new List<Person>
        {
            new Person { UniqueId = "ENTOCR13RSCLZ6KU", FirstName = "John", LastName = "Doe", Language = "English", Version = 1.0 },
             new Person { UniqueId = "V59OF92YF627HFY0", FirstName = "Jane", LastName = "Doe", Language = "English", Version = 1.0 },
            new Person { UniqueId = "5ZVOEPMJUI4MB4EN", FirstName = "Jane", LastName = "Smith", Language = "English", Version = 1.0 }
        };

            await PersonContext.Persons.AddRangeAsync(persons);
            await PersonContext.SaveChangesAsync();

            // Act
            var sortedPersons = await PersonRepository.GetSortedPersonsAsync();

            // Assert           
            Assert.Multiple(() =>
            {
                Assert.That(sortedPersons.Count, Is.EqualTo(3));
                Assert.That(sortedPersons[0].LastName, Is.EqualTo("Doe"));
                Assert.That(sortedPersons[1].LastName, Is.EqualTo("Doe"));
                Assert.That(sortedPersons[2].LastName, Is.EqualTo("Smith"));
                Assert.That(sortedPersons[0].FirstName, Is.EqualTo("Jane"));
                Assert.That(sortedPersons[1].FirstName, Is.EqualTo("John"));
                Assert.That(sortedPersons[2].FirstName, Is.EqualTo("Jane"));
            });
           
        }


    }
}
