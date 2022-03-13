using bm.core;
using bm.core.cache;
using bm.core.exceptions;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;

namespace bm.test.core;

[TestFixture]
public class TestCache
{
    static Filter _cached = new()
    {
        City = "Cached",
    };
    static Filter _provider = new()
    {
        City = "Provider"
    };
    static Filter _exception = new()
    {
        City = "Exception"
    };
    static Filter _null = new()
    {
        City = "null"
    };

    static IEnumerable<MakelaarStatistic> _cachedResult = new List<MakelaarStatistic>()
    {
        new() { MakelaarID = 42, Name = "Foo", PropertyCount = 12 },
        new() { MakelaarID = 12, Name = "Bar", PropertyCount = 8 },
        new() { MakelaarID = 67, Name = "Buzz", PropertyCount = 16 },
    };

    static IEnumerable<MakelaarStatistic> _providerResult = new List<MakelaarStatistic>()
    {
        new() { MakelaarID = 42, Name = "Foo", PropertyCount = 67 },
        new() { MakelaarID = 12, Name = "Bar", PropertyCount = 234 },
        new() { MakelaarID = 67, Name = "Buzz", PropertyCount = 24 },
    };

    private Mock<IStatisticsCache>? _statsCache { get; set; }
    private Mock<IStatisticsCalculator>? _statsCalculator { get; set; }

    private Statistics? _cacheSvc { get; set; }

    [SetUp]
    public void SetUp()
    {
        var loggerFactory = new NullLoggerFactory();
        var nopLogger = loggerFactory.CreateLogger<Statistics>();

        _statsCache = new Mock<IStatisticsCache>();
        _statsCache.Setup(_ => _.TryGetValue(It.Is<string>(x => x == _cached.ToString()), out _cachedResult)).Returns(true);

        _statsCalculator = new Mock<IStatisticsCalculator>();
        _statsCalculator.Setup(_ => _.Calculate(It.Is<Filter>(x => x.ToString() == _provider.ToString()))).ReturnsAsync(_providerResult);

        _statsCalculator.Setup(_ => _.Calculate(It.Is<Filter>(x => x.ToString() == _exception.ToString()))).ThrowsAsync(new Exception("mock exception"));

        IEnumerable<MakelaarStatistic> nil = null;
        _statsCalculator.Setup(_ => _.Calculate(It.Is<Filter>(x => x.ToString() == _null.ToString()))).ReturnsAsync(nil);

        _cacheSvc = new Statistics(nopLogger, _statsCache.Object, _statsCalculator.Object);
    }

    [Test]
    public async Task TestGetStatistics_Cached()
    {
        if (_cacheSvc == null || _statsCalculator == null)
        {
            Assert.Fail();
            return;
        }

        var got = await _cacheSvc.GetStatistics(_cached);
        got.Should().BeEquivalentTo(_cachedResult);

        _statsCalculator.Verify(mock => mock.Calculate(_cached), Times.Never);
    }

    [Test]
    public async Task TestGetStatistics_ViaProvider()
    {
        if (_cacheSvc == null || _statsCache == null)
        {
            Assert.Fail();
            return;
        }

        var got = await _cacheSvc.GetStatistics(_provider);
        got.Should().BeEquivalentTo(_providerResult);

        _statsCache.Verify(mock => mock.Set(_provider.ToString(), _providerResult), Times.Once);
    }

    [Test]
    public async Task TestGetStatistics_Errors()
    {
        if (_cacheSvc == null || _statsCalculator == null)
        {
            Assert.Fail();
            return;
        }

        Assert.ThrowsAsync<StatisticsProvisionException>(() => _cacheSvc.GetStatistics(_exception));
        Assert.ThrowsAsync<NullResultException>(() => _cacheSvc.GetStatistics(_null));

        _statsCalculator.Verify(mock => mock.Calculate(_cached), Times.Never);
    }
}