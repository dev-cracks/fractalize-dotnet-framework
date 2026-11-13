namespace Efc.Fractalize.Ddd;

public record DateRange : ValueObjectBase
{
    public DateTimeOffset Start { get; private set; }
    public DateTimeOffset End { get; private set; }

    private DateRange()
    {
    }

    public DateRange(DateTimeOffset startDate, DateTimeOffset endDate)
    {
        if (startDate == default)
        {
            throw new ArgumentNullException(nameof(startDate));
        }

        if (endDate == default)
        {
            throw new ArgumentNullException(nameof(endDate));
        }

        if (endDate <= startDate)
        {
            throw new ArgumentOutOfRangeException($"{nameof(endDate)} should be higher than {startDate}");
        }

        Start = startDate;
        End = endDate;
    }

    public bool Overlaps(DateRange dateRange) =>
        Start < dateRange.End && End > dateRange.Start;

    public int DurationInMinutesRounded() => (int)DurationInMinutes();

    public double DurationInMinutes() => (End - Start).TotalMinutes;
}
