using StackExchange.Redis;
using System.Text.Json;

namespace OrderService.Api.Data
{
    public class RedisCacheService
    {
        private readonly IConnectionMultiplexer _redis;
        public RedisCacheService(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        public async Task SetCacheAsync<T>(string key, T value, TimeSpan expiration)
        {
            var db = _redis.GetDatabase();
            var jsonData = JsonSerializer.Serialize(value);
            await db.StringSetAsync(key, jsonData, expiration);
        }

        public async Task<T?> GetCacheAsync<T>(string key)
        {
            var db = _redis.GetDatabase();
            var jsonData = await db.StringGetAsync(key);
            return jsonData.IsNullOrEmpty ? default : JsonSerializer.Deserialize<T>(jsonData);
        }
        public async Task RemoveCacheAsync(string key)
        {
            var db = _redis.GetDatabase();
            await db.KeyDeleteAsync(key);
        }
    }
}
