using System;
using bm.core;
using NUnit.Framework;

namespace bm.test.core;

[TestFixture]
public class TestFilter
{
    static object[] filterCases = {
        new Object[] { new Filter {
            City = "",
            FeatureFilter = FeatureFilter.Garden,
        }, "Purchase/Garden" },
        new Object[] { new Filter {
            City = "Amsterdam",
            FeatureFilter = FeatureFilter.Garden,
        }, "Purchase/Amsterdam/Garden" },
        new Object[] { new Filter {
            City = "Amsterdam",
            FeatureFilter = FeatureFilter.None,
        }, "Purchase/Amsterdam"}
    };

    [Test]
    [TestCaseSource(nameof(filterCases))]
    public void TestToString(Filter testCase, string exp)
    {
        var got = testCase.ToString();
        Assert.AreEqual(exp, got);
    }
}