using BoostLingo.Core;
using BoostLingo.Interfaces;
using BoostLingo.Utility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;

namespace BoostLingo.Business
{
    public class DataManager : IDataManager
    {
        private readonly ILogger<DataManager> Logger;
        private readonly IHttpClientFactory HttpClientFactory;
        private readonly string DataUrl;
        private readonly int MaxRetryCount;
        private readonly int RetryIntervalInSeconds;

        public DataManager(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<DataManager> logger)
        {           
            HttpClientFactory = httpClientFactory;
            Logger = logger;
            DataUrl = configuration["UrlSettings:Url"] ?? Constants.DefaultDataUrl;
            MaxRetryCount = Convert.ToInt32(configuration["MaxRetryCount"] ?? Constants.DefalutRetryCount);
            RetryIntervalInSeconds = Convert.ToInt32(configuration["RetryIntervalInSeconds"] ?? Constants.DefaultRetryIntervalInSeconds);

        }
        public async Task<List<Person>> FetchDataAsync()
        {
            List<Person> persons = new List<Person>();
            try
            {
                var rawData = await Retry.RetryAsync(GetRawData, MaxRetryCount, TimeSpan.FromSeconds(RetryIntervalInSeconds));
                persons = rawData.Any() ? ConvertRawDataToPerson(rawData) : new List<Person>();

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                throw;
            }

            return persons;
        }

        private async Task<List<RawPerson>> GetRawData()
        {
            List<RawPerson> rawData = new List<RawPerson>();
            Logger.LogInformation("Start retrieving data from the file");
            try
            {
                var httpClient = HttpClientFactory.CreateClient();
                HttpResponseMessage response = await httpClient.GetAsync(DataUrl);
                response.EnsureSuccessStatusCode();
                var data = await response.Content.ReadAsStringAsync();
                rawData = data != null ? JsonConvert.DeserializeObject<List<RawPerson>>(data) : rawData;
                Logger.LogInformation("Successfully retrieved  data");
                
            }
            catch (HttpRequestException ex)
            {
                Logger.LogCritical($"Error while fetching data: {ex.Message}");
                throw;
            }
            catch (Newtonsoft.Json.JsonException ex)
            {
                Logger.LogCritical($"Error while parsing JSON: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Logger.LogCritical($"Unexpected error while fetching data from url: {ex.Message}");
                throw;
            }

            return rawData;
        }

        private List<Person> ConvertRawDataToPerson(List<RawPerson> rawData)
        {
           var persons = new List<Person>();
            foreach (var rawPerson in rawData)
            {
                try
                {
                    var nameParts = rawPerson.Name.Split(' ', 2);
                    var person = new Person
                    {
                        UniqueId = rawPerson.Id,
                        FirstName = nameParts.Length > 0 ? nameParts[0] : string.Empty,
                        LastName = nameParts.Length > 1 ? nameParts[1] : string.Empty,
                        Language = rawPerson.Language,
                        Version = rawPerson.Version,
                        PersonBio = new PersonBio { BioText = rawPerson.Bio }
                    };
                    persons.Add(person);
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Error converting data with Id: {rawPerson.Id}");
                    Logger.LogError($"Error Message: {ex.Message}");
                }
            }
            return persons;

        }
    }
}

