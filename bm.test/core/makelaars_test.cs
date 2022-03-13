using NUnit.Framework;
using bm.core;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using FluentAssertions;
using System.Threading.Tasks;

namespace bm.test.core;

[TestFixture]
public class MakelaarsServiceTests
{
    [Test]
    public async Task TestFetchBusiestMakelaars()
    {
        var loggerFactory = new NullLoggerFactory();
        var nopLogger = loggerFactory.CreateLogger<MakelaarsService>();

        var moqStatProvider = new Mock<IStatisticsProvider>();
        moqStatProvider.Setup(_ => _.GetStatistics(It.IsAny<Filter>()))
            .ReturnsAsync(new List<MakelaarStatistic>()
            {
                new() { MakelaarID = 1, Name = "Alpha", PropertyCount = 17 },
                new() { MakelaarID = 2, Name = "Beta", PropertyCount = 12 },
                new() { MakelaarID = 3, Name = "Gamma", PropertyCount = 18 },
                new() { MakelaarID = 4, Name = "Delta", PropertyCount = 5 },
                new() { MakelaarID = 5, Name = "Epsilon", PropertyCount = 26 },
            });

        var svc = new MakelaarsService(nopLogger, moqStatProvider.Object);
        var got = await svc.FetchBusiestMakelaars(new Filter(), 2);

        var exp = new List<MakelaarStatistic>();
        exp.Add(new() { MakelaarID = 5, Name = "Epsilon", PropertyCount = 26 });
        exp.Add(new() { MakelaarID = 3, Name = "Gamma", PropertyCount = 18 });

        got.Should().BeEquivalentTo(exp);
    }
}