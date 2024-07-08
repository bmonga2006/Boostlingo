using BoostLingo.Business;
using BoostLingo.Core;
using BoostLingo.Interfaces;
using BoostLingo.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace BoostLingo
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();   
            ConfigureServices(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var dataManager = serviceProvider.GetService<IDataManager>();
            var personRepository = serviceProvider.GetService<IPersonRepository>();
            var Logger = serviceProvider.GetService<ILogger<Program>>();

            try
            {
                Console.WriteLine($"");
                List<Person> persons = await dataManager.FetchDataAsync();
                if(persons != null && persons.Any())
                {
                    await personRepository.AddPersonsAsync(persons);
                    List<Person> sortedPersons = await personRepository.GetSortedPersonsAsync();
                    DisplayPersons(sortedPersons);
                }            

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to process data: {ex.Message}");
                Logger.LogCritical($"Failed to process data: {ex.Message}");
            }


        }

        private static void DisplayPersons(List<Person> persons)
        {
            foreach (var person in persons)
            {
                Console.WriteLine($"UniqueId: {person.UniqueId}, Name: {person.LastName}, {person.FirstName}, Language: {person.Language}, Version: {person.Version}");
                if (person.PersonBio != null)
                {
                    Console.WriteLine($"\tBio: {person.PersonBio.BioText}");
                }
                else
                {
                    Console.WriteLine($"\tBio: {string.Empty}");
                }
            }
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            string connectionString = configuration.GetConnectionString("DefaultConnection");


            services.AddSingleton<IConfiguration>(configuration);

            services.AddHttpClient<IDataManager, DataManager>(client =>
            {
                client.Timeout = TimeSpan.FromSeconds(30);
            });

            services.AddDbContext<PersonContext>(options =>
                options.UseSqlServer(connectionString));

          

            services.AddTransient<IDataManager, DataManager>();
            services.AddTransient<IPersonRepository, PersonRepository>();

            services.AddLogging(builder => builder.AddSerilog(new LoggerConfiguration()
                                                  .ReadFrom.Configuration(configuration)
                                                 .CreateLogger()));
                                              
        }
    }
}
