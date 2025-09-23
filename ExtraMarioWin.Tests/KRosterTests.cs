using System;
using ExtraMarioWin;
using Xunit;

namespace ExtraMarioWin.Tests;

public class KRosterTests
{
    [Fact]
    public void Count_Empty_IsZero()
    {
        var roster = new KRoster();
        Assert.Equal(0, roster.Count());
    }

    [Fact]
    public void Add_IncreasesCount()
    {
        var roster = new KRoster();
        roster.Add(new KSinger(Guid.NewGuid(), "Alpha"));
        Assert.Equal(1, roster.Count());
    }

    [Fact]
    public void Get_ValidIndex_ReturnsSinger()
    {
        var roster = new KRoster();
        var singer = new KSinger(Guid.NewGuid(), "Beta");
        roster.Add(singer);
        var fetched = roster.Get(0);
        Assert.NotNull(fetched);
        Assert.Equal(singer.id, fetched!.id);
        Assert.Equal(singer.stageName, fetched.stageName);
    }

    [Fact]
    public void Get_InvalidIndex_ReturnsNull()
    {
        var roster = new KRoster();
        Assert.Null(roster.Get(0));
        roster.Add(new KSinger(Guid.NewGuid(), "Gamma"));
        Assert.Null(roster.Get(-1));
        Assert.Null(roster.Get(1));
    }
}