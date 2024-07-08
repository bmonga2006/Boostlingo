using BoostLingo.Core;

namespace BoostLingo.Interfaces
{
    public interface IDataManager
    {
        Task<List<Person>> FetchDataAsync();
    }
}
