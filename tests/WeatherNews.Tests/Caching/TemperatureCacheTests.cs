using Microsoft.Extensions.Caching.Hybrid;
using Moq;
using WeatherNews.Application.Abstractions;
using WeatherNews.Domain.Entities;
using WeatherNews.Domain.Enums;
using WeatherNews.Infrastructure.Caching;

namespace WeatherNews.Tests.Caching;

/*
*   TESTUJEME:
*
*   - správny key
*   - TTL výpočet
*   - ukladanie do HybridCache
*/

public class TemperatureCacheTests
{
    [Fact]
    public async Task Stores_value_with_correct_key()
    {
        var cache = new Mock<HybridCache>();
        var logger = Mock.Of<Microsoft.Extensions.Logging.ILogger<TemperatureCache>>();
        var clock = new Mock<IDateTimeProvider>();

        clock.Setup(c => c.UtcNow).Returns(new DateTime(2026, 1, 1, 8, 0, 0, DateTimeKind.Utc));

        var sut = new TemperatureCache(cache.Object, logger, clock.Object);

        var reading = new TemperatureReading(CityId.Bratislava, 10, "", DateTime.UtcNow);

        await sut.SetAsync(reading);

        cache.Verify(c => c.SetAsync(
            "temperature:1",
            reading,
            It.IsAny<HybridCacheEntryOptions>(),
            default),
            Times.Once);
    }
}

