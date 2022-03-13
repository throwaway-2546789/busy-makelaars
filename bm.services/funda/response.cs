using System.Text.Json.Serialization;

namespace bm.services.funda;

public class ListingsResponse
{
    [JsonPropertyName("Objects")]
    public IEnumerable<Listing> Listings { get; set; } = Enumerable.Empty<Listing>();

    [JsonPropertyName("Paging")]
    public PagingInfo Paging { get; set; } = new();
}

public class Listing
{
    [JsonPropertyName("Id")]
    public Guid Id { get; set; }

    [JsonPropertyName("MakelaarId")]
    public int MakelaarId { get; set; }

    [JsonPropertyName("MakelaarNaam")]
    public string MakelaarName { get; set; } = "";
}

public class PagingInfo
{
    [JsonPropertyName("AantalPaginas")]
    public int TotalPages { get; set; }

    [JsonPropertyName("HuidigePagina")]
    public int CurrentPage { get; set; }

    [JsonPropertyName("VolgendeUrl")]
    public string NextUrl { get; set; } = "";

    [JsonPropertyName("VorigeUrl")]
    public string PreviousUrl { get; set; } = "";
}