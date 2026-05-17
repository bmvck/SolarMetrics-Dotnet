using Microsoft.AspNetCore.Http;
using SolarMetrics.Common;
using Xunit;

namespace SolarMetrics.UnitTests.Common;

public sealed class HateoasLinkBuilderTests
{
    [Fact]
    public void ForCollection_PaginaDoMeio_IncluiNextEPrev()
    {
        var context = new DefaultHttpContext();
        context.Request.Scheme = "http";
        context.Request.Host = new HostString("localhost", 5090);

        var links = HateoasLinkBuilder.ForCollection(
            context.Request,
            "/Cliente",
            page: 2,
            pageSize: 10,
            totalPages: 5,
            new Dictionary<string, string?>());

        Assert.Contains(links, l => l.Rel == "self");
        Assert.Contains(links, l => l.Rel == "next");
        Assert.Contains(links, l => l.Rel == "prev");
        Assert.Contains(links, l => l.Rel == "first");
        Assert.Contains(links, l => l.Rel == "last");
    }

    [Fact]
    public void ForItem_IncluiSelfUpdateDelete()
    {
        var context = new DefaultHttpContext();
        context.Request.Scheme = "https";
        context.Request.Host = new HostString("api.example.com");
        var id = Guid.NewGuid();

        var links = HateoasLinkBuilder.ForItem(context.Request, "/Cliente", id);

        Assert.Contains(links, l => l.Rel == "self" && l.Href.Contains(id.ToString()));
        Assert.Contains(links, l => l.Rel == "update");
        Assert.Contains(links, l => l.Rel == "delete");
    }
}
