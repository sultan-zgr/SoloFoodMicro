using StackExchange.Redis;
using System.Text.Json;
using Serilog;

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
            try
            {
                var db = _redis.GetDatabase();
                var jsonData = JsonSerializer.Serialize(value);
                await db.StringSetAsync(key, jsonData, expiration);
                Log.Information($"Cache set for key: {key}");
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to set cache for key {key}: {ex.Message}");
                throw;  // Hata varsa yeniden fırlat
            }
        }

        public async Task<T?> GetCacheAsync<T>(string key)
        {
            try
            {
                var db = _redis.GetDatabase();
                var jsonData = await db.StringGetAsync(key);

                if (jsonData.IsNullOrEmpty)
                {
                    Log.Information($"Cache miss for key: {key}");
                    return default;
                }

                Log.Information($"Cache hit for key: {key}");
                return JsonSerializer.Deserialize<T>(jsonData);
            }
            catch (JsonException ex)
            {
                Log.Error($"Failed to deserialize cache data for key {key}: {ex.Message}");
                return default;  // Deserialize hatası olursa null döner
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to get cache for key {key}: {ex.Message}");
                throw;  // Diğer hataları fırlat
            }
        }

        public async Task RemoveCacheAsync(string key)
        {
            try
            {
                var db = _redis.GetDatabase();
                await db.KeyDeleteAsync(key);
                Log.Information($"Cache removed for key: {key}");
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to remove cache for key {key}: {ex.Message}");
                throw;  // Hata varsa yeniden fırlat
            }
        }
    }
}
