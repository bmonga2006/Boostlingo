using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoostLingo.Utility
{
    public static class Retry
    {
        public static async Task<T> RetryAsync<T>(Func<Task<T>> func, int maxRetries, TimeSpan retryInterval)
        {
            for (var i = 0; i < maxRetries; i++)
            {
                try
                {
                   return await func();
                    
                }
                catch when (i < maxRetries)
                {
                    await Task.Delay(retryInterval);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Failed {i + 1}: Getting Exception : {ex.Message}");
                }
            }
            throw new InvalidOperationException("This code should not be reached.");

        }

        public static async Task RetryAsync<TParam>(Func<TParam, Task> func, TParam param, int maxRetries, TimeSpan retryInterval)
        {
            for (var i = 0; i < maxRetries; i++)
            {
                try
                {
                    await func(param);
                    break;
                }
                catch when (i < maxRetries)
                {
                    await Task.Delay(retryInterval);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Failed {i + 1}: Getting Exception : {ex.Message}");
                }
            }
        }

    }
}
