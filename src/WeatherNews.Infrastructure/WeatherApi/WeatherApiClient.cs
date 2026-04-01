using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using WeatherNews.Application.Abstractions;
using WeatherNews.Application.Common;
using WeatherNews.Domain.Constants;
using WeatherNews.Domain.Entities;
using WeatherNews.Domain.Enums;

namespace WeatherNews.Infrastructure.WeatherApi;

public sealed class WeatherApiClient(HttpClient httpClient, ILogger<WeatherApiClient> logger)
    : IWeatherProvider
{
    public async Task<Result<TemperatureReading>> GetTemperatureAsync(
        CityId cityId,
        CancellationToken cancellationToken = default)
    {
        int Id = CityConstants.CityIds[cityId];

        logger.LogInformation("Requesting temperature for {City} (WeatherAPI cityId={CityId})", cityId, Id);

        HttpResponseMessage response;

        try
        {
            response = await httpClient.GetAsync($"/{Id}", cancellationToken);
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
            logger.LogWarning("WeatherAPI returned 404 for city {City} (cityId={CityId})", cityId, Id);
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

        if (payload is null)
        {
            logger.LogWarning("WeatherAPI returned null or empty JSON for {City}", cityId);
            return Result<TemperatureReading>.Failure("Invalid Weather API response.");
        }

        var reading = new TemperatureReading(
            cityId,
            Math.Round((decimal)payload.TemperatureC, 2),
            payload.MeasuredAtUtc);

        logger.LogInformation(
            "WeatherAPI returned {Temperature}°C for {City} at {TimestampUtc}",
            reading.TemperatureC,
            cityId,
            reading.MeasuredAtUtc);

        return Result<TemperatureReading>.Success(reading);
    }

    private sealed class WeatherApiResponse
    {
        public double TemperatureC { get; set; }
        public DateTime MeasuredAtUtc { get; set; }
    }
}
