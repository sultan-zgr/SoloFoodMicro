using StackExchange.Redis;
using System.Text.Json;

namespace OrderService.Api.Data
{
    public class RedisCacheService
    {
        private readonly IDatabase _database;

        public RedisCacheService(IConnectionMultiplexer redis)
        {
            _database = redis.GetDatabase();
        }

        public async Task<bool> SetCacheAsync<T>(string key, T value, TimeSpan expiration)
        {
            var jsonData = JsonSerializer.Serialize(value);
            return await _database.StringSetAsync(key, jsonData, expiration);
        }

        public async Task<T?> GetCacheAsync<T>(string key)
        {
            var jsonData = await _database.StringGetAsync(key);
            return jsonData.IsNullOrEmpty ? default : JsonSerializer.Deserialize<T>(jsonData);
        }
    }
}
