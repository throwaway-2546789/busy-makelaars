using bm.core;
using bm.core.exceptions;
using System.Net.Http.Json;

namespace bm.services.funda;

public class FundaService : IListingsProvider
{
    private readonly HttpClient _httpClient;

    public FundaService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<core.Listing>> GetListings(Filter filter, CancellationToken cancellationToken = default)
    {
        return (await RequestListings(filter, cancellationToken)).Select(x => new core.Listing
        {
            Id = x.Id,
            MakelaarId = x.MakelaarId,
            MakelaarName = x.MakelaarName
        });
    }

    public async Task<IEnumerable<Listing>> RequestListings(Filter filter, CancellationToken cancellationToken = default)
    {
        var result = new List<Listing>();

        try
        {
            result = await makeListingsRequests(filter, 1, result, cancellationToken);
        }
        catch (Exception ex)
        {
            throw new ListingsRequestException(ex);
        }

        return result;
    }

    private async Task<List<Listing>> makeListingsRequests(Filter filter, int page, List<Listing> listings, CancellationToken cancellationToken = default)
    {
        var qs = new Query(filter).ToQuery(page);
        if (_httpClient.BaseAddress == null)
        {
            throw new ConfigurationException("funda.httpClient.BaseAddress");
        }

        var reqUri = new Uri(_httpClient.BaseAddress, qs);

        var result = await _httpClient.GetFromJsonAsync<ListingsResponse>(reqUri, cancellationToken);
        if (result == null)
        {
            throw new NullResultException();
        }

        listings.AddRange(result.Listings);
        if (string.IsNullOrEmpty(result.Paging.NextUrl))
        {
            return listings;
        }

        return await makeListingsRequests(filter, page + 1, listings, cancellationToken);
    }
}
