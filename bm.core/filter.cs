using System.Text;

namespace bm.core;

public class Filter
{
    public string City { get; set; } = "";
    public FeatureFilter FeatureFilter { get; set; } = FeatureFilter.None;
    public ListingType ListingType { get; set; } = ListingType.Purchase;

    public override string ToString()
    {
        var strBuilder = new StringBuilder();
        strBuilder.Append(this.ListingType.ToString());

        if (!string.IsNullOrEmpty(this.City))
        {
            strBuilder.Append($"/{this.City}");
        }

        if (this.FeatureFilter != FeatureFilter.None)
        {
            strBuilder.Append($"/{this.FeatureFilter.ToString()}");
        }

        return strBuilder.ToString();
    }
}

public enum FeatureFilter
{
    None,
    Garden // etc can be extended
}

public enum ListingType
{
    Purchase,
    Rental // etc maybe
}