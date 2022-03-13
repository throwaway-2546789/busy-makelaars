using Microsoft.Extensions.Logging;

namespace bm.core.statistics;

public class StatisticsCalculator : IStatisticsCalculator
{
    private readonly ILogger<StatisticsCalculator> _logger;

    private readonly IListingsProvider _listingsProvider;

    public StatisticsCalculator(ILogger<StatisticsCalculator> logger, IListingsProvider listingsProvider)
    {
        _logger = logger;
        _listingsProvider = listingsProvider;
    }

    public async Task<IEnumerable<MakelaarStatistic>> Calculate(Filter filter)
    {
        _logger.LogInformation("forming makelaar statistics based on available listings");
        var listings = await _listingsProvider.GetListings(filter);

        return listings.GroupBy(x => x.MakelaarId).Select(gr => new MakelaarStatistic
        {
            MakelaarID = gr.Key,
            Name = gr.First().MakelaarName,
            PropertyCount = gr.Count()
        });
    }
}