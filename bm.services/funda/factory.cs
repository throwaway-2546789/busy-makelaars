namespace bm.services.funda;

public class Config
{
    public string BaseUrl { get; set; } = "http://partnerapi.funda.nl/feeds/Aanbod.svc/json/";
    public string Key { get; set; } = "";
}

public class Factory
{
    private readonly Config _config;
    public Factory(Config config)
    {
        _config = config;
    }

    public FundaService Create()
    {
        var b = new Uri(_config.BaseUrl);
        var baseUri = new Uri(b, _config.Key);

        var httpClient = new HttpClient()
        {
            BaseAddress = baseUri
        };

        return new FundaService(httpClient);
    }
}