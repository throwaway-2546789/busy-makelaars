using Microsoft.Extensions.Caching.Memory;

namespace bm.core.cache;

public interface IStatisticsCache
{
    bool TryGetValue(string key, out IEnumerable<MakelaarStatistic> value);
    IEnumerable<MakelaarStatistic> Set(string key, IEnumerable<MakelaarStatistic> value);
}

public class StatisticsCache : IStatisticsCache
{
    private readonly IMemoryCache _memoryCache;
    public StatisticsCache(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public IEnumerable<MakelaarStatistic> Set(string key, IEnumerable<MakelaarStatistic> value)
    {
        return _memoryCache.Set(key, value);
    }

    public bool TryGetValue(string key, out IEnumerable<MakelaarStatistic> value)
    {
        return _memoryCache.TryGetValue(key, out value);
    }
}
