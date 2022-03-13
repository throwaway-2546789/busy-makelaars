using System.Collections.Specialized;
using System.Text;

namespace bm.services.funda;

public class Query
{
    private readonly core.Filter _filter;
    public Query(core.Filter filter)
    {
        _filter = filter;
    }

    public string ToQuery(int page = 1, int pageSize = 25)
    {
        var qs = new NameValueCollection();

        var t = _filter.ListingType == core.ListingType.Purchase ? "koop" : "huur";
        qs.Add("type", t);

        var sb = new StringBuilder();
        sb.Append($"/{_filter.City.ToLower()}/");

        if (_filter.FeatureFilter == core.FeatureFilter.Garden)
        {
            sb.Append($"tuin/");
        }

        qs.Add("zo", sb.ToString());
        qs.Add("page", page.ToString());
        qs.Add("pagesize", pageSize.ToString());

        return "?" + string.Join("&", qs.AllKeys.Select(x => $"{x}={qs[x]}"));
    }
}