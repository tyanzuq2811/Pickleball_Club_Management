using System.Text.Json;
using PCM.Application.Interfaces;
using StackExchange.Redis;

namespace PCM.Infrastructure.Services;

public class RedisService : IRedisService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IDatabase _db;

    public RedisService(IConnectionMultiplexer redis)
    {
        _redis = redis;
        _db = redis.GetDatabase();
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var value = await _db.StringGetAsync(key);
        
        if (!value.HasValue)
            return default;

        return JsonSerializer.Deserialize<T>((string)value!);
    }

    public async Task<bool> SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        var serializedValue = JsonSerializer.Serialize(value);
        return await _db.StringSetAsync(key, serializedValue, expiry ?? TimeSpan.FromMinutes(60));
    }

    public async Task<bool> DeleteAsync(string key)
    {
        return await _db.KeyDeleteAsync(key);
    }

    public async Task<bool> ExistsAsync(string key)
    {
        return await _db.KeyExistsAsync(key);
    }

    public async Task<bool> SetAddAsync(string key, string value)
    {
        return await _db.SetAddAsync(key, value);
    }

    public async Task<long> SetRemoveAsync(string key, string value)
    {
        return await _db.SetRemoveAsync(key, value) ? 1 : 0;
    }

    public async Task<IEnumerable<string>> SetMembersAsync(string key)
    {
        var members = await _db.SetMembersAsync(key);
        return members.Select(m => m.ToString());
    }

    public async Task<bool> SortedSetAddAsync(string key, string member, double score)
    {
        return await _db.SortedSetAddAsync(key, member, score);
    }

    public async Task<bool> SortedSetRemoveAsync(string key, string member)
    {
        return await _db.SortedSetRemoveAsync(key, member);
    }

    public async Task<IEnumerable<(string member, double score)>> SortedSetRangeByRankWithScoresAsync(
        string key, long start = 0, long stop = -1, bool descending = true)
    {
        var order = descending ? Order.Descending : Order.Ascending;
        var entries = await _db.SortedSetRangeByRankWithScoresAsync(key, start, stop, order);
        
        return entries.Select(e => (e.Element.ToString(), e.Score));
    }
}
