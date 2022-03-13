using Microsoft.Extensions.Logging;
using bm.core.exceptions;

namespace bm.core.cache;

public class Statistics : IStatisticsProvider
{
    private readonly ILogger<Statistics> _logger;
    private readonly IStatisticsCache _statsCache;
    private readonly IStatisticsProvider _statsProvider;

    public Statistics(
        ILogger<Statistics> logger,
        IStatisticsCache statsCache,
        IStatisticsProvider statsProvider
    )
    {
        _logger = logger;
        _statsCache = statsCache;
        _statsProvider = statsProvider;
    }

    public async Task<IEnumerable<MakelaarStatistic>> GetStatistics(Filter filter)
    {
        var filterKey = filter.ToString();
        _logger.LogInformation("Attempting retrieval of {filterKey} statistics", filterKey);

        IEnumerable<MakelaarStatistic> result;
        if (_statsCache.TryGetValue(filterKey, out result) && result != null)
        {
            _logger.LogInformation("Successfully retrieved {filterKey} from in memory cache", filterKey);
            return result;
        }

        _logger.LogInformation("No stats related to {filterKey} in cache, attempting retrieval from provider", filterKey);
        try
        {
            result = await _statsProvider.GetStatistics(filter);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception thrown retrieving statistics from provider");
            throw new StatisticsProvisionException(ex);
        }

        if (result == null)
        {
            _logger.LogError("Statistics provider returned null");
            throw new NullResultException();
        }

        _statsCache.Set(filterKey, result);
        return result;
    }
}