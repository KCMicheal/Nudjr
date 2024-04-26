using Nudjr_AppCore.Services.CacheServices.Dtos;
using Nudjr_AppCore.Services.CacheServices.Interfaces;
using Nudjr_AppCore.Services.Shared.Interfaces;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nudjr_AppCore.Services.CacheServices.Implementation
{
   public class CacheService : ICacheService
   {
       private readonly IDatabase _redis;
       private readonly IServer _server;
       private ILoggerManager _logger;

       public CacheService(IConnectionMultiplexer redis, ILoggerManager loggerManager)
       {
           _redis = redis.GetDatabase();
           _server = redis.GetServer(redis.GetEndPoints()[0]);
           _logger = loggerManager;
       }

       public async Task<bool> WriteToCache<T>(string key, T payload, CacheKeySets? cacheKeySets, TimeSpan? absoluteExpireTime)
       {
           try
           {
               string stringifiedJson = JsonConvert.SerializeObject(payload);
               bool isSet = await _redis.StringSetAsync(key, stringifiedJson, absoluteExpireTime, When.Always).ConfigureAwait(true);

               if (cacheKeySets.HasValue)
               {
                   isSet = await _redis.SetAddAsync(cacheKeySets.Value.ToString(), key);
               }

               return isSet;
           }
           catch (Exception ex)
           {
               _logger.LogError($"[WriteToCacheAsync]=> Key: {key} | {JsonConvert.SerializeObject(ex)}");
           }

           return false;
       }

       public async Task<T?> ReadFromCache<T>(string key) where T : class
       {
           try
           {
               string? serializedPayload = await _redis.StringGetAsync(key: key);
               if (!string.IsNullOrWhiteSpace(serializedPayload))
               {
                   T? payload = JsonConvert.DeserializeObject<T>(serializedPayload);
                   return payload;
               }
           }
           catch (Exception ex)
           {
               _logger.LogError($"[ReadFromCacheAsync]=> Key: {key} | {JsonConvert.SerializeObject(ex)}");
           }
            
           return default(T);
       }

       public async Task ClearFromCache(string key)
       {
           try
           {
               bool _isKeyExist = _redis.KeyExists(key);
               if (_isKeyExist == true)
               {
                   await _redis.KeyDeleteAsync(key);
               }
           }
           catch (Exception ex)
           {
               _logger.LogError($"[ClearFromCacheAsync]=> Key: {key} | {JsonConvert.SerializeObject(ex)}");
           }
            
       }

       public async Task ClearFromCache(CacheKeySets cacheKeySets, string key)
       {
           await _redis.KeyDeleteAsync(key);
           await _redis.SetRemoveAsync(cacheKeySets.ToString(), key);
       }

       public async Task<IEnumerable<T>> BulkReadFromCache<T>(string pattern) where T : class
       {
           List<T> records = new List<T>();
           foreach (var key in _server.Keys(pattern: pattern))
           {
               T? record = await ReadFromCache<T>(key.ToString());
               if (record != default(T))
               {
                   records.Add(record);
               }
           }
           return records;
       }

       public async Task<IEnumerable<T>> BulkReadFromCache<T>(CacheKeySets cacheKeySets) where T : class
       {
           List<T> records = new List<T>();

           var keys = await _redis.SetMembersAsync(cacheKeySets.ToString());

           foreach (var key in keys)
           {
               T? record = await ReadFromCache<T>(key.ToString());
               if (record != default(T))
               {
                   records.Add(record);
               }
               else
               {
                   await _redis.SetRemoveAsync(cacheKeySets.ToString(), key);
               }
           }
           return records;
       }
   }
}
