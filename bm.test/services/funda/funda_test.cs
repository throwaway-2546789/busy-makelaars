using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using bm.services.funda;
using FluentAssertions;
using Moq;
using Moq.Protected;
using NUnit.Framework;

namespace bm.test.services.funda;

[TestFixture]
public class TestFundaService
{
    [Test]
    public async Task TestRequestListings()
    {
        var exp = new List<Listing>();
        exp.AddRange(_resOne.Listings);
        exp.AddRange(_resTwo.Listings);
        exp.AddRange(_resThree.Listings);

        var filter = new bm.core.Filter { City = "Amsterdam" };

        var handlerMock = new Mock<HttpMessageHandler>();
        setupMock(handlerMock, "page=1", _resOne);
        setupMock(handlerMock, "page=2", _resTwo);
        setupMock(handlerMock, "page=3", _resThree);

        var httpClient = new HttpClient(handlerMock.Object)
        {
            BaseAddress = new Uri("https://example.com"),
        };

        var svc = new FundaService(httpClient);
        var got = await svc.RequestListings(filter);

        got.Should().BeEquivalentTo(exp);
    }

    private void setupMock(Mock<HttpMessageHandler> handlerMock, string pageContains, ListingsResponse expResult)
    {
        try
        {
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(x => x.RequestUri.ToString().Contains(pageContains)),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(expResult))
                })
                .Verifiable();
        }
        catch (Exception ex)
        {
            Assert.Fail(ex.Message);
        }
    }

    static ListingsResponse _resOne = new()
    {
        Listings = new List<Listing> {
            new() { Id = Guid.NewGuid(), MakelaarId = 1, MakelaarName = "Foo" },
            new() { Id = Guid.NewGuid(), MakelaarId = 2, MakelaarName = "Bar" },
            new() { Id = Guid.NewGuid(), MakelaarId = 5, MakelaarName = "Buzz" },
        },
        Paging = new PagingInfo
        {
            CurrentPage = 1,
            NextUrl = "something",
        }
    };
    static ListingsResponse _resTwo = new()
    {
        Listings = new List<Listing> {
            new() { Id = Guid.NewGuid(), MakelaarId = 5, MakelaarName = "Buzz" },
            new() { Id = Guid.NewGuid(), MakelaarId = 2, MakelaarName = "Bar" },
            new() { Id = Guid.NewGuid(), MakelaarId = 5, MakelaarName = "Buzz" },
        },
        Paging = new PagingInfo
        {
            CurrentPage = 2,
            NextUrl = "something",
        }
    };
    static ListingsResponse _resThree = new()
    {
        Listings = new List<Listing> {
            new() { Id = Guid.NewGuid(), MakelaarId = 1, MakelaarName = "Foo" },
            new() { Id = Guid.NewGuid(), MakelaarId = 2, MakelaarName = "Bar" },
        },
        Paging = new PagingInfo
        {
            CurrentPage = 3,
            NextUrl = "",
        }
    };
}