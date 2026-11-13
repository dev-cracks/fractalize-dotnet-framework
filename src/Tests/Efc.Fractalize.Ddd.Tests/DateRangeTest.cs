namespace Efc.Fractalize.Ddd.Tests;

public class DateRangeTest
{
    private static readonly DateTimeOffset Start = TestClock.Now;

    [Fact]
    public void Constructor_WithAValidRange_KeepsStartAndEnd()
    {
        var range = new DateRange(Start, Start.AddHours(2));

        Assert.Equal(Start, range.Start);
        Assert.Equal(Start.AddHours(2), range.End);
    }

    // Materializers such as EF Core use the private parameterless constructor.
    [Fact]
    public void PrivateConstructor_LeavesDefaultDates()
    {
        var range = (DateRange)Activator.CreateInstance(typeof(DateRange), nonPublic: true)!;

        Assert.Equal(default, range.Start);
        Assert.Equal(default, range.End);
    }

    [Fact]
    public void Constructor_WithStartDateHigherThanEnd_ThrowsArgumentOutOfRangeException() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => new DateRange(Start.AddDays(1), Start.AddDays(-1)));

    [Fact]
    public void Constructor_WithTheSameStartAndEnd_ThrowsArgumentOutOfRangeException() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => new DateRange(Start, Start));

    [Fact]
    public void Constructor_WithDefaultDates_ThrowsArgumentNullExceptionNamingTheParameter()
    {
        var startDateDefault = Assert.Throws<ArgumentNullException>(() => new DateRange(default, Start));
        Assert.Equal("startDate", startDateDefault.ParamName);

        var endDateDefault = Assert.Throws<ArgumentNullException>(() => new DateRange(Start, default));
        Assert.Equal("endDate", endDateDefault.ParamName);
    }

    [Fact]
    public void DurationInMinutes_WithAValidRange_ReturnsTheIntervalInMinutes()
    {
        var range = new DateRange(Start, Start.AddMinutes(90.5));

        Assert.Equal(90.5, range.DurationInMinutes());
    }

    [Fact]
    public void DurationInMinutesRounded_WithAValidRange_TruncatesTheMinutes()
    {
        var range = new DateRange(Start, Start.AddMinutes(90.9));

        Assert.Equal(90, range.DurationInMinutesRounded());
    }

    [Fact]
    public void StartAndEnd_ModifiedByEncapsulation_ShouldNotChange()
    {
        var range = new DateRange(Start, Start.AddDays(1));

        _ = range.Start.AddDays(100);
        _ = range.End.AddYears(-100);

        Assert.Equal(Start, range.Start);
        Assert.Equal(Start.AddDays(1), range.End);
    }

    [Fact]
    public void Equals_WithTheSameStartAndEnd_ReturnsTrue()
    {
        var range = new DateRange(Start, Start.AddHours(2));
        var same = new DateRange(Start, Start.AddHours(2));

        Assert.Equal(range, same);
        Assert.True(range == same);
        Assert.Equal(range.GetHashCode(), same.GetHashCode());
        Assert.NotEqual(range, new DateRange(Start, Start.AddHours(3)));
    }

    [Fact]
    public void Overlaps_WithARangeThatStartsInside_ReturnsTrue()
    {
        var range = new DateRange(Start, Start.AddHours(2));
        var other = new DateRange(Start.AddHours(1), Start.AddHours(3));

        Assert.True(range.Overlaps(other));
    }

    // Regression test: Overlaps used to ignore the end of the other range, so a range that ends
    // before this one starts was reported as overlapping.
    [Theory]
    [InlineData(-3, -2)]
    [InlineData(-1, 0)]
    public void Overlaps_WithARangeBeforeThisOne_ReturnsFalse(int startHours, int endHours)
    {
        var range = new DateRange(Start, Start.AddHours(2));
        var earlier = new DateRange(Start.AddHours(startHours), Start.AddHours(endHours));

        Assert.False(range.Overlaps(earlier));
        Assert.False(earlier.Overlaps(range));
    }

    [Fact]
    public void Overlaps_WithARangeThatContainsIt_ReturnsTrue()
    {
        var range = new DateRange(Start, Start.AddHours(2));
        var wider = new DateRange(Start.AddHours(-1), Start.AddHours(3));

        Assert.True(range.Overlaps(wider));
        Assert.True(wider.Overlaps(range));
    }

    [Fact]
    public void Overlaps_WithARangeAfterThisOne_ReturnsFalse()
    {
        var range = new DateRange(Start, Start.AddHours(2));
        var later = new DateRange(Start.AddHours(3), Start.AddHours(4));

        Assert.False(range.Overlaps(later));
    }

    [Fact]
    public void Overlaps_WithARangeThatStartsAtTheEnd_ReturnsFalse()
    {
        var range = new DateRange(Start, Start.AddHours(2));
        var adjacent = new DateRange(Start.AddHours(2), Start.AddHours(3));

        Assert.False(range.Overlaps(adjacent));
    }
}
