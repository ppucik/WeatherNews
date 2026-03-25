using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using WeatherNews.Application.Abstractions;
using WeatherNews.Application.Common;
using WeatherNews.Application.Temperature;
using WeatherNews.Domain.Entities;
using WeatherNews.Domain.Enums;

namespace WeatherNews.Tests.Temperature;

/*
*   TESTUJEME:
*
*   - cache hit
*   - cache miss → API success
*   - cache miss → API failure
*   - správne volanie portov
*/

public class TemperatureServiceTests
{
    private readonly Mock<IWeatherProvider> _provider = new();
    private readonly Mock<ITemperatureCache> _cache = new();
    private readonly Mock<IDateTimeProvider> _clock = new();
    private readonly Mock<ILogger<TemperatureService>> _logger = new();

    [Fact]
    public async Task Returns_cached_value_when_available()
    {
        var reading = new TemperatureReading(CityId.Bratislava, 10, DateTime.UtcNow);
        _cache.Setup(c => c.GetAsync(CityId.Bratislava, default)).ReturnsAsync(reading);

        var sut = new TemperatureService(_provider.Object, _cache.Object, _clock.Object, _logger.Object);

        var result = await sut.GetCurrentTemperatureAsync(CityId.Bratislava);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(reading);

        _provider.Verify(p => p.GetTemperatureAsync(It.IsAny<CityId>(), default), Times.Never);
    }

    [Fact]
    public async Task Calls_provider_and_caches_result_when_not_cached()
    {
        _cache.Setup(c => c.GetAsync(CityId.Bratislava, default)).ReturnsAsync((TemperatureReading?)null);

        var reading = new TemperatureReading(CityId.Bratislava, 12, DateTime.UtcNow);
        _provider.Setup(p => p.GetTemperatureAsync(CityId.Bratislava, default))
                 .ReturnsAsync(Result<TemperatureReading>.Success(reading));

        var sut = new TemperatureService(_provider.Object, _cache.Object, _clock.Object, _logger.Object);

        var result = await sut.GetCurrentTemperatureAsync(CityId.Bratislava);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(reading);

        _cache.Verify(c => c.SetAsync(reading, default), Times.Once);
    }

    [Fact]
    public async Task Returns_failure_when_provider_fails()
    {
        _cache.Setup(c => c.GetAsync(CityId.Bratislava, default)).ReturnsAsync((TemperatureReading?)null);

        _provider.Setup(p => p.GetTemperatureAsync(CityId.Bratislava, default))
                 .ReturnsAsync(Result<TemperatureReading>.Failure("fail"));

        var sut = new TemperatureService(_provider.Object, _cache.Object, _clock.Object, _logger.Object);

        var result = await sut.GetCurrentTemperatureAsync(CityId.Bratislava);

        result.IsSuccess.Should().BeFalse();
    }
}

