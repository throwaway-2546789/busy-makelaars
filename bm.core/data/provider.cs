using Microsoft.Extensions.Logging;
using bm.core.exceptions;

namespace bm.core.data;

public class Provider : IStatisticsProvider
{
    private readonly ILogger<Provider> _logger;
    private readonly IStatisticsCache _statsCache;
    private readonly IStatisticsCalculator _statsCalculator;

    public Provider(
        ILogger<Provider> logger,
        IStatisticsCache statsCache,
        IStatisticsCalculator statsCalculator
    )
    {
        _logger = logger;
        _statsCache = statsCache;
        _statsCalculator = statsCalculator;
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
            result = await _statsCalculator.Calculate(filter);
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