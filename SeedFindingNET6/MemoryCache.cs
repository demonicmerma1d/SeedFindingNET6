using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeedFindingNET6
{
    //not thread safe
    public class MemoryCache<T>
    {
        public double SlidingExpiration;
        public double AbsoluteExpiration;
        public MemoryCache(double sliding,double absolute) 
        {
            SlidingExpiration = sliding;
            AbsoluteExpiration = absolute;
        }
        private MemoryCache _cache = new(new MemoryCacheOptions()
        {
            SizeLimit = 1024
        });

        public T GetOrCreate(object key, Func<T> createItem)
        {
            T cacheEntry;
            if (!_cache.TryGetValue(key, out cacheEntry))// Look for cache key.
            {
                // Key not in cache, so get data.
                cacheEntry = createItem();

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                 .SetSize(1)//Size amount
                            //Priority on removing when reaching size limit (memory pressure)
                    .SetPriority(CacheItemPriority.High)
                    // Keep in cache for this time, reset time if accessed.
                    .SetSlidingExpiration(TimeSpan.FromSeconds(SlidingExpiration))
                    // Remove from cache after this time, regardless of sliding expiration
                    .SetAbsoluteExpiration(TimeSpan.FromSeconds(AbsoluteExpiration));

                // Save data in cache.
                _cache.Set(key, cacheEntry, cacheEntryOptions);
            }
            return cacheEntry;
        }
    }

}
