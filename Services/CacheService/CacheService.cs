namespace SocialNetwork.Services.CacheService
{
    public class CacheService : ICacheService
    {
        private readonly IDatabase _cache;
        public CacheService(IConfiguration configuration)
        {
            var redis = ConnectionMultiplexer.Connect(configuration.GetConnectionString("RedisConnection"));
            _cache = redis.GetDatabase();
        }
        public async Task<T> GetData<T>(string key)
        {
            var value = await _cache.StringGetAsync(key);
            if(!string.IsNullOrEmpty(value)) {
                return JsonSerializer.Deserialize<T>(value);
            }
            return default;
        }

        public async Task<bool> SetData<T>(string key, T value, DateTimeOffset expirationTime)
        {
            var expiryTime = expirationTime.DateTime.Subtract(DateTime.Now);
            return await _cache.StringSetAsync(key, JsonSerializer.Serialize(value), expiryTime);
        }
    }
}
