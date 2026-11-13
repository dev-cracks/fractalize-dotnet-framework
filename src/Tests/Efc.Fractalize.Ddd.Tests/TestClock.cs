namespace Efc.Fractalize.Ddd.Tests;

/// <summary>Fixed instant so that the tests never depend on the real clock.</summary>
internal static class TestClock
{
    public static readonly DateTimeOffset Now = new(2026, 1, 1, 10, 0, 0, TimeSpan.Zero);
}
