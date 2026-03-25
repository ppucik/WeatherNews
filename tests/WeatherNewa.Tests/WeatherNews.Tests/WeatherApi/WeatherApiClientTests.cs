using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using WeatherNews.Domain.Enums;
using WeatherNews.Infrastructure.WeatherApi;
using WeatherNews.Tests.TestHelpers;

namespace WeatherNews.Tests.WeatherApi;

/*
*   TESTUJEME:
*
*   - 200 OK → valid JSON
*   - 404 → failure
*   - 500 → failure
*   - timeout
*   - invalid JSON
*/

public class WeatherApiClientTests
{
    private WeatherApiClient CreateClient(HttpResponseMessage response)
    {
        var handler = new FakeHttpMessageHandler(_ => response);
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("http://test") };
        var logger = Mock.Of<ILogger<WeatherApiClient>>();

        return new WeatherApiClient(httpClient, logger);
    }

    [Fact]
    public async Task Returns_success_on_valid_response()
    {
        var json = """
        { "temperatureC": 10.5, "measuredAtUtc": "2024-01-01T10:00:00Z" }
        """;

        var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
        {
            Content = new StringContent(json)
        };

        var client = CreateClient(response);

        var result = await client.GetTemperatureAsync(CityId.Bratislava);

        result.IsSuccess.Should().BeTrue();
        result.Value!.TemperatureC.Should().Be(10.5m);
    }

    [Fact]
    public async Task Returns_failure_on_404()
    {
        var response = new HttpResponseMessage(System.Net.HttpStatusCode.NotFound);

        var client = CreateClient(response);

        var result = await client.GetTemperatureAsync(CityId.Bratislava);

        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task Returns_failure_on_invalid_json()
    {
        var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
        {
            Content = new StringContent("not json")
        };

        var client = CreateClient(response);

        var result = await client.GetTemperatureAsync(CityId.Bratislava);

        result.IsSuccess.Should().BeFalse();
    }
}
