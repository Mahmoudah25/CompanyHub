using CompanyHub.Application.Common.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CompanyHub.Infrastructure.Cache
{
    public class CacheSerivce : ICacheService
    {
        private readonly IDistributedCache cache;
        public CacheSerivce(IDistributedCache cache)
        {
            this.cache = cache;
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            var cashedData = await cache.GetStringAsync(key);
            if(string.IsNullOrEmpty(cashedData))
                return default;
            return JsonSerializer.Deserialize<T>(cashedData);
        }

        //public async Task GetOne<T>(string key)
        //{
        //    var caheddata= await cache.GetStringAsync(key);
        //    return JsonSerializer.Deserialize(caheddata);
        //}

        public async Task RemoveAsync(string key)
        {
            await cache.RemoveAsync(key);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan expiration)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration
            };

            var json = JsonSerializer.Serialize(value);
            await cache.SetStringAsync(key, json,options);
        }
    }
}
