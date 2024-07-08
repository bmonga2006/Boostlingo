using BoostLingo.Core;

namespace BoostLingo.Interfaces
{
    public interface IPersonRepository
    {
      

        Task AddPersonsAsync(List<Person> persons);

        Task<List<Person>> GetSortedPersonsAsync();
    }
}
