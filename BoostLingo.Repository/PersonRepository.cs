using BoostLingo.Core;
using BoostLingo.Interfaces;
using BoostLingo.Utility;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;


namespace BoostLingo.Repository
{
    public class PersonRepository : IPersonRepository
    {
        private readonly PersonContext PersonContext;
        private readonly ILogger<PersonRepository> Logger;
        private readonly int BatchSize;
        private readonly int MaxRetryCount;
        private readonly int RetryIntervalInSeconds;

        public PersonRepository(PersonContext personContext, IConfiguration configuration, ILogger<PersonRepository> logger)
        {
            PersonContext = personContext;
            Logger = logger;
            BatchSize = Convert.ToInt32(configuration["BatchSize"] ?? Constants.DefaultBatchSize);
            MaxRetryCount = Convert.ToInt32(configuration["MaxRetryCount"] ?? Constants.DefalutRetryCount);
            RetryIntervalInSeconds = Convert.ToInt32(configuration["RetryIntervalInSeconds"] ?? Constants.DefaultRetryIntervalInSeconds);
        }        

        public async Task AddPersonsAsync(List<Person> persons)
        {
            for (int i = 0; i < persons.Count; i += BatchSize)
            {
                var batch = persons.Skip(i).Take(BatchSize).ToList();
                try
                {
                    await Retry.RetryAsync(ProcessBatch, batch, MaxRetryCount, TimeSpan.FromSeconds(RetryIntervalInSeconds));
                }

                catch (Exception ex)
                {

                    Logger.LogError($"Error processing batch starting at index {i}: {ex.Message}");
                }
               

            }
        }

        private async Task ProcessBatch(List<Person> persons)
        {
            try
            {
                await PersonContext.Persons.AddRangeAsync(persons);
                await PersonContext.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                throw;
            }
        }

       public async Task<List<Person>> GetSortedPersonsAsync()
       {
            try
            {
                Logger.LogInformation("Build Query to get sorted persons from Db");
                IQueryable<Person> query = PersonContext.Persons
                                               .Include(p => p.PersonBio)
                                               .OrderBy(p => p.LastName)
                                               .ThenBy(p => p.FirstName);

                Logger.LogInformation("Retrieving sorted persons from the database.");
                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error while getting sorted persons from database: {ex.Message}");
                throw;
            }
       }
    }
}
