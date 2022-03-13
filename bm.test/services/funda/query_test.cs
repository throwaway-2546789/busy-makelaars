using bm.core;
using bm.services.funda;
using NUnit.Framework;

namespace bm.test.services.funda;

[TestFixture]
public class TestQuery
{
    static object[] _testCaseSource = {
        new object[] { new Filter {
            City = "Amsterdam",
            FeatureFilter = FeatureFilter.Garden
        }, "?type=koop&zo=/amsterdam/tuin/&page=1&pagesize=25" },
        new object[] { new Filter {
            City = "Haarlem",
            FeatureFilter = FeatureFilter.None,
            ListingType = ListingType.Rental
        }, "?type=huur&zo=/haarlem/&page=1&pagesize=25" },
    };

    [Test]
    [TestCaseSource(nameof(_testCaseSource))]
    public void TestToString(Filter testcase, string exp)
    {
        var got = new Query(testcase).ToQuery(page: 1, pageSize: 25);
        Assert.AreEqual(exp, got);
    }
}