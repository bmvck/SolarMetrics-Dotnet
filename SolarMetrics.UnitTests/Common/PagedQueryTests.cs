using SolarMetrics.Common;
using Xunit;

namespace SolarMetrics.UnitTests.Common;

public sealed class PagedQueryTests
{
    [Fact]
    public void Normalize_PageZero_DefineComo1()
    {
        var query = new PagedQuery { Page = 0, PageSize = 20 };
        query.Normalize();
        Assert.Equal(1, query.Page);
    }

    [Fact]
    public void Normalize_PageSizeAcimaDoMaximo_LimitaA100()
    {
        var query = new PagedQuery { Page = 1, PageSize = 500 };
        query.Normalize(100);
        Assert.Equal(100, query.PageSize);
    }

    [Fact]
    public void Normalize_SortDirInvalido_DefineAsc()
    {
        var query = new PagedQuery { SortDir = "invalid" };
        query.Normalize();
        Assert.Equal("asc", query.SortDir);
    }
}
