using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CacheAPI.Service
{
    public interface ICacheService
    {
        Task<bool> WriteDataToCache(Dictionary<string, object> objects );
        Dictionary<string, object> ReadDataFromCache( List<string> keys );
        Task<List<string>> ReadAllKeysFromCache();

        Task<string> RemoveKeysFromCache(List<string> keys);

        Task<List<string>> ReadAllKeysByToken(string token);
    }
}
