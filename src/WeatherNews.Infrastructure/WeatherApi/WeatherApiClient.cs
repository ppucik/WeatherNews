using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using WeatherNews.Application.Abstractions;
using WeatherNews.Application.Common;
using WeatherNews.Domain.Entities;
using WeatherNews.Domain.Enums;
using WeatherNews.Infrastructure.Configuration;

namespace WeatherNews.Infrastructure.WeatherApi;

public sealed class WeatherApiClient(HttpClient httpClient,
    IOptions<WeatherApiOptions> options,
    ILogger<WeatherApiClient> logger)
    : IWeatherProvider
{
    private readonly WeatherApiOptions _settings = options.Value;

    public async Task<Result<TemperatureReading>> GetTemperatureAsync(
        CityId cityId,
        CancellationToken cancellationToken = default)
    {
        string cityName = cityId.ToString();

        logger.LogInformation("Requesting temperature for {City} (WeatherAPI cityId={CityId})", cityId, cityName);

        HttpResponseMessage response;

        try
        {
            var queryUrl = $"current.json?key={_settings.ApiKey}&q={Uri.EscapeDataString(cityName)}&aqi=no";
            logger.LogInformation("Calling WeatherAPI with URL: {Url}", queryUrl);
            response = await httpClient.GetAsync(queryUrl, cancellationToken);
        }
        catch (TaskCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            logger.LogWarning("WeatherAPI request for {City} timed out", cityId);
            return Result<TemperatureReading>.Failure("Weather API timeout.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error while calling WeatherAPI for {City}", cityId);
            return Result<TemperatureReading>.Failure("Weather API error.");
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            logger.LogWarning("WeatherAPI returned 404 for city {City}", cityId);
            return Result<TemperatureReading>.Failure("City not found.");
        }

        if (!response.IsSuccessStatusCode)
        {
            logger.LogWarning("WeatherAPI returned non-success status {StatusCode} for {City}", (int)response.StatusCode, cityId);
            return Result<TemperatureReading>.Failure("Weather API error.");
        }

        WeatherApiResponse? payload;

        try
        {
            payload = await response.Content.ReadFromJsonAsync<WeatherApiResponse>(cancellationToken: cancellationToken);
        }
        catch (JsonException ex)
        {
            logger.LogWarning(ex, "WeatherAPI returned invalid JSON for {City}", cityId);
            return Result<TemperatureReading>.Failure("Invalid Weather API response.");
        }

        if (payload is null || payload.Current is null || payload.Current.Condition is null)
        {
            logger.LogWarning("WeatherAPI returned payload missing required fields for {City}", cityId);
            return Result<TemperatureReading>.Failure("Invalid Weather API response.");
        }

        var reading = new TemperatureReading(
            cityId,
            Math.Round((decimal)payload.Current.TempC, 2),
            payload.Current.Condition.Text,
            DateTime.Parse(payload.Current.LastUpdated));

        logger.LogInformation(
            "WeatherAPI returned {Temperature}°C for {City} at {TimestampUtc}",
            reading.TemperatureC,
            cityId,
            reading.MeasuredAtUtc);

        return Result<TemperatureReading>.Success(reading);
    }
}
