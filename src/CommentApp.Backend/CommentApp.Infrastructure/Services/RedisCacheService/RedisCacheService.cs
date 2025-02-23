namespace CommentApp.Infrastructure.Services.RedisCacheService;

public class RedisCacheService(IConnectionMultiplexer redis) : ICacheService
{
    private readonly IDatabase _database = redis.GetDatabase();

    public void TestConnection()
    {
        if (redis.IsConnected)
        {
            Console.WriteLine("Redis is connected!");
        }
        else
        {
            Console.WriteLine("Redis is NOT connected!");
        }
    }

    public async Task SetAsync(string key, string value, TimeSpan expiration)
    {
        await _database.StringSetAsync(key, value, expiration);
    }

    public async Task<string?> GetAsync(string key)
    {
        return await _database.StringGetAsync(key);
    }

    public async Task RemoveAsync(string key)
    {
        await _database.KeyDeleteAsync(key);
    }
}