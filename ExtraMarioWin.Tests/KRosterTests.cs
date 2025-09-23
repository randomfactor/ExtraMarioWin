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

    [Fact]
    public void Remove_ExistingSinger_DecreasesCount()
    {
        var roster = new KRoster();
        var singer = new KSinger(Guid.NewGuid(), "Delta");
        roster.Add(singer);
        Assert.True(roster.Remove(singer));
        Assert.Equal(0, roster.Count());
    }

    [Fact]
    public void Remove_Null_ReturnsFalse()
    {
        var roster = new KRoster();
        Assert.False(roster.Remove(null!));
    }

    [Fact]
    public void Remove_NotInRoster_ReturnsFalse()
    {
        var roster = new KRoster();
        var singer1 = new KSinger(Guid.NewGuid(), "Echo");
        var singer2 = new KSinger(Guid.NewGuid(), "Foxtrot");
        roster.Add(singer1);
        Assert.False(roster.Remove(singer2));
        Assert.Equal(1, roster.Count());
    }

    [Fact]
    public void Bump_WithTwoOrMore_SwapsFirstTwo()
    {
        var roster = new KRoster();
        var first = new KSinger(Guid.NewGuid(), "First");
        var second = new KSinger(Guid.NewGuid(), "Second");
        var third = new KSinger(Guid.NewGuid(), "Third");
        roster.Add(first);
        roster.Add(second);
        roster.Add(third);

        var result = roster.Bump();

        Assert.True(result);
        Assert.Equal(second.id, roster.Get(0)!.id);
        Assert.Equal(first.id, roster.Get(1)!.id);
        // Ensure third unchanged
        Assert.Equal(third.id, roster.Get(2)!.id);
    }

    [Fact]
    public void Bump_WithZeroOrOneSinger_NoOpReturnsFalse()
    {
        var rosterEmpty = new KRoster();
        Assert.False(rosterEmpty.Bump());

        var rosterOne = new KRoster();
        var only = new KSinger(Guid.NewGuid(), "Solo");
        rosterOne.Add(only);
        Assert.False(rosterOne.Bump());
        Assert.Equal(only.id, rosterOne.Get(0)!.id);
    }

    // New tests for Next()
    [Fact]
    public void Next_Empty_ReturnsFalse()
    {
        var roster = new KRoster();
        Assert.False(roster.Next());
    }

    [Fact]
    public void Next_Single_ReturnsTrue_NoChange()
    {
        var roster = new KRoster();
        var only = new KSinger(Guid.NewGuid(), "Only");
        roster.Add(only);
        Assert.True(roster.Next());
        Assert.Equal(only.id, roster.Get(0)!.id);
        Assert.Equal(1, roster.Count());
    }

    [Fact]
    public void Next_Multiple_MovesFirstToEnd()
    {
        var roster = new KRoster();
        var a = new KSinger(Guid.NewGuid(), "A");
        var b = new KSinger(Guid.NewGuid(), "B");
        var c = new KSinger(Guid.NewGuid(), "C");
        roster.Add(a);
        roster.Add(b);
        roster.Add(c);

        Assert.True(roster.Next());
        Assert.Equal(b.id, roster.Get(0)!.id);
        Assert.Equal(c.id, roster.Get(1)!.id);
        Assert.Equal(a.id, roster.Get(2)!.id);
    }
}