namespace PCM.Application.Interfaces;

public interface IRedisService
{
    Task<T?> GetAsync<T>(string key);
    Task<bool> SetAsync<T>(string key, T value, TimeSpan? expiry = null);
    Task<bool> DeleteAsync(string key);
    Task<bool> ExistsAsync(string key);
    Task<bool> SetAddAsync(string key, string value);
    Task<long> SetRemoveAsync(string key, string value);
    Task<IEnumerable<string>> SetMembersAsync(string key);
    Task<bool> SortedSetAddAsync(string key, string member, double score);
    Task<bool> SortedSetRemoveAsync(string key, string member);
    Task<IEnumerable<(string member, double score)>> SortedSetRangeByRankWithScoresAsync(string key, long start = 0, long stop = -1, bool descending = true);
}
