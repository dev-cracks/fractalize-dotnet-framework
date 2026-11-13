namespace Efc.Fractalize.Ddd;

/// <summary>
/// Value object for a period of time from <see cref="Start"/> (included) to <see cref="End"/> (excluded).
/// </summary>
/// <remarks>
/// Both dates are required and <see cref="End"/> is always later than <see cref="Start"/>. Two ranges are
/// equal when both dates are equal (record equality).
/// </remarks>
public record DateRange : ValueObjectBase
{
    /// <summary>
    /// Start of the range, included.
    /// </summary>
    public DateTimeOffset Start { get; private set; }

    /// <summary>
    /// End of the range, excluded.
    /// </summary>
    public DateTimeOffset End { get; private set; }

    private DateRange()
    {
    }

    /// <summary>
    /// Creates a range.
    /// </summary>
    /// <param name="startDate">Start of the range, included.</param>
    /// <param name="endDate">End of the range, excluded; later than <paramref name="startDate"/>.</param>
    /// <exception cref="ArgumentNullException">A date is the default value.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="endDate"/> is not later than <paramref name="startDate"/>.
    /// </exception>
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

    /// <summary>
    /// Determines whether this range and another one share any instant. Ranges that only touch (one ends
    /// when the other starts) do not overlap.
    /// </summary>
    /// <param name="dateRange">The other range.</param>
    /// <returns><see langword="true"/> when each range starts before the other one ends.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="dateRange"/> is <see langword="null"/>.</exception>
    public bool Overlaps(DateRange dateRange)
    {
        ArgumentNullException.ThrowIfNull(dateRange);
        return Start < dateRange.End && End > dateRange.Start;
    }

    /// <summary>
    /// Duration in whole minutes; the fraction is truncated.
    /// </summary>
    /// <returns>The number of whole minutes between <see cref="Start"/> and <see cref="End"/>.</returns>
    public int DurationInMinutesRounded() => (int)DurationInMinutes();

    /// <summary>
    /// Duration in minutes, with the fraction.
    /// </summary>
    /// <returns>The minutes between <see cref="Start"/> and <see cref="End"/>.</returns>
    public double DurationInMinutes() => (End - Start).TotalMinutes;
}
