using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using WeatherNews.Domain.Enums;
using WeatherNews.Infrastructure.Configuration;
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
        var options = Options.Create(new WeatherApiOptions
        {
            BaseUrl = "http://test",
            ApiKey = "test-api-key"
        });

        return new WeatherApiClient(httpClient, options, logger);
    }

    [Fact]
    public async Task Returns_success_on_valid_response()
    {
        // Opravený JSON: Musí obsahovať štruktúru 'current' a pole 'temp_c'
        var json = """
    {
        "location": { "name": "Bratislava" },
        "current": {
            "temp_c": 10.5,
            "condition": { "text": "Clear" },
            "last_updated": "2024-01-01 10:00"
        }
    }
    """;

        var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
        {
            Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
        };

        var client = CreateClient(response);

        // Ak váš klient vracia doménový model, uistite sa, že mapper vnútri klienta 
        // správne spracoval temp_c na TemperatureC
        var result = await client.GetTemperatureAsync(CityId.Bratislava);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
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
