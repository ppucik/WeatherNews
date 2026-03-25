using FluentAssertions;
using WeatherNews.Infrastructure.Caching;

namespace WeatherNews.Tests.Time;

public class TemperatureCacheTimeTests
{
    [Fact]
    public void Before9_ReturnsTimeUntil9()
    {
        var now = new DateTime(2024, 1, 1, 8, 0, 0, DateTimeKind.Utc);

        var result = TemperatureCacheTime.CalculateTtlUntilNextChange(now);

        result.Should().Be(TimeSpan.FromHours(1));
    }

    [Fact]
    public void ExactlyAt9_ReturnsTimeUntil16()
    {
        var now = new DateTime(2024, 1, 1, 9, 0, 0, DateTimeKind.Utc);

        var result = TemperatureCacheTime.CalculateTtlUntilNextChange(now);

        result.Should().Be(TimeSpan.FromHours(7));
    }

    [Fact]
    public void Between9And16_ReturnsTimeUntil16()
    {
        var now = new DateTime(2024, 1, 1, 12, 30, 0, DateTimeKind.Utc);

        var result = TemperatureCacheTime.CalculateTtlUntilNextChange(now);

        result.Should().Be(TimeSpan.FromHours(3.5));
    }

    [Fact]
    public void ExactlyAt16_ReturnsTimeUntilNextDay9()
    {
        var now = new DateTime(2024, 1, 1, 16, 0, 0, DateTimeKind.Utc);

        var result = TemperatureCacheTime.CalculateTtlUntilNextChange(now);

        result.Should().Be(TimeSpan.FromHours(17)); // 16:00 → next day 9:00
    }

    [Fact]
    public void After16_ReturnsTimeUntilNextDay9()
    {
        var now = new DateTime(2024, 1, 1, 20, 0, 0, DateTimeKind.Utc);

        var result = TemperatureCacheTime.CalculateTtlUntilNextChange(now);

        result.Should().Be(TimeSpan.FromHours(13)); // 20:00 → next day 9:00
    }

    [Fact]
    public void JustBeforeMidnight_ReturnsTimeUntilNextDay9()
    {
        var now = new DateTime(2024, 1, 1, 23, 59, 0, DateTimeKind.Utc);

        var result = TemperatureCacheTime.CalculateTtlUntilNextChange(now);

        result.Should().Be(TimeSpan.FromHours(9).Add(TimeSpan.FromMinutes(1)));
    }

    [Fact]
    public void ExactlyAtMidnight_ReturnsTimeUntil9()
    {
        var now = new DateTime(2024, 1, 2, 0, 0, 0, DateTimeKind.Utc);

        var result = TemperatureCacheTime.CalculateTtlUntilNextChange(now);

        result.Should().Be(TimeSpan.FromHours(9));
    }
}
