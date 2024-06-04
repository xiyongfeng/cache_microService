using Logging.Extensions.Logging;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CacheAPI.Service
{
    public class CacheService : ICacheService
    {
        private static ILogger _logger = AppLogger.LoggerFactory.CreateLogger<CacheService>();

        private static ConcurrentDictionary<string, object> _cacheData = new ConcurrentDictionary<string, object>();

        public CacheService ()
        {
        }

        public async Task<List<string>> ReadAllKeysFromCache()
        {
            if (_cacheData == null) return null;
            var keys = await Task.Run(() => _cacheData.Keys.ToList<string>());

            return keys;
        }

        public async Task<List<string>> ReadAllKeysByToken( string token )
        {
            string msg = null;

            if (string.IsNullOrEmpty(token))
                return null;

            if (_cacheData == null) return null;
            var keys = await Task.Run(() => _cacheData.Keys.ToList<string>());

            keys = keys.Where(k => k.StartsWith(token)).ToList();

            return keys;
        }


        public Dictionary<string, object> ReadDataFromCache(List<string> keys)
        {
            if (keys == null) return null;

            var data = new Dictionary<string, object>();
            foreach( var key in keys)
            {
                if(_cacheData.ContainsKey(key))
                {
                    object value = null;
                    var success = _cacheData.TryGetValue(key, out value);

                    if(success)  data.Add(key, value);
                    else
                    {
                        _logger.LogError(new EventId(7, "Cache"), $"reading cache failed for account {key}");
                    }
                }
            }

            return data;
        }

        public async Task<bool> WriteDataToCache(Dictionary<string, object> objects)
        {
            if (objects == null || !objects.Any())
                return false;

            if(_cacheData == null )
                return false;

            var hasError = await Task.Run(()=> WritingTocache(objects));
          
            return hasError;
        }

        public async Task<string> RemoveKeysFromCache(List<string> keys)
        {
            if (keys == null || !keys.Any())
                return "Cache remove keys with empty lsit.";

            if (_cacheData == null)
                return "Cache store is empty.";

            var hasError = await Task.Run(() => RemovingKeysFromcache(keys));

            return !hasError ? "Success" : "Failure";
        }
        


        private bool WritingTocache(Dictionary<string, object> objects)
        {
            bool success = true;
            foreach (var item in objects)
            {
                object oldobj = null;
                if (_cacheData.ContainsKey(item.Key))
                    _cacheData.TryRemove(item.Key, out oldobj);

                //adding new one
                if (!_cacheData.TryAdd(item.Key, item.Value))
                {
                    success = false;
                    _logger.LogError(new EventId(7, "Cache"), $"writing to cache failed for account {item.Key}");
                }
            }

            return success;
        }

        private bool RemovingKeysFromcache( List<string> keys )
        {
            bool hasError = false;
            
            if(keys != null && keys.Count==1 && keys[0]!= null && keys[0].ToLower()=="allkeys" )
            {
                _cacheData.Clear();
                return true;
            }
            
            foreach (var key in keys)
            {
                object oldobj = null;
                if (_cacheData.ContainsKey(key))
                {
                  if(  !_cacheData.TryRemove(key, out oldobj) )
                    {
                        hasError = true;
                    }
                }
            }

            return hasError;
        }
    }
}
