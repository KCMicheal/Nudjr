using Nudjr_AppCore.Services.CacheServices.Dtos;

namespace Nudjr_AppCore.Services.CacheServices.Interfaces
{
    public interface ICacheService
    {
        Task ClearFromCache(string key);

        Task ClearFromCache(CacheKeySets cacheKeySets, string key);
        Task<bool> WriteToCache<T>(string key, T payload, CacheKeySets? cacheKeySets = null, TimeSpan? absoluteExpireTime = null);

        Task<T?> ReadFromCache<T>(string key) where T : class;

        Task<IEnumerable<T>> BulkReadFromCache<T>(CacheKeySets cacheKeySets) where T : class;

        Task<IEnumerable<T>> BulkReadFromCache<T>(string pattern) where T : class;
    }
}
