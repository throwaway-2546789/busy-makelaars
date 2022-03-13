using Microsoft.Extensions.Logging;

namespace bm.core;

public interface IBusyMakelaars
{
    public Task<IEnumerable<MakelaarStatistic>> FetchBusiestMakelaars(Filter filter, int count);
}

public interface IStatisticsProvider
{
    public Task<IEnumerable<MakelaarStatistic>> GetStatistics(Filter filter);
}

public interface IListingsProvider
{
    public Task<IEnumerable<Listing>> GetListings(Filter filter, CancellationToken cancellationToken = default!);
}

public class MakelaarStatistic
{
    public int MakelaarID { get; set; }
    public string Name { get; set; } = "";
    public int PropertyCount { get; set; }
}

public class Listing
{
    public Guid Id { get; set; }
    public int MakelaarId { get; set; }
    public string MakelaarName { get; set; } = "";
}

public class MakelaarsService : IBusyMakelaars
{
    private readonly ILogger<MakelaarsService> _logger;
    private readonly IStatisticsProvider _statsProvider;

    public MakelaarsService(ILogger<MakelaarsService> logger, IStatisticsProvider statsProvider)
    {
        _logger = logger;
        _statsProvider = statsProvider;
    }

    public async Task<IEnumerable<MakelaarStatistic>> FetchBusiestMakelaars(Filter filter, int count)
    {
        _logger.LogInformation("Fetching a list of busiest makelaars");

        var stats = await _statsProvider.GetStatistics(filter);
        return stats.OrderByDescending(x => x.PropertyCount).Take(count);
    }
}
