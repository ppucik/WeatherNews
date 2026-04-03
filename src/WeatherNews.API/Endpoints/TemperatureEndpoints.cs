using Microsoft.AspNetCore.Mvc;
using System.Collections.Frozen;
using WeatherNews.Application.Abstractions;
using WeatherNews.Domain.Enums;

namespace WeatherNews.API.Endpoints;

public static class TemperatureEndpoints
{
    private static readonly FrozenDictionary<string, CityId> CityMap =
        new Dictionary<string, CityId>(StringComparer.OrdinalIgnoreCase)
        {
            [nameof(CityId.Bratislava)] = CityId.Bratislava,
            [nameof(CityId.Praha)] = CityId.Praha,
            [nameof(CityId.Budapest)] = CityId.Budapest,
            [nameof(CityId.Vieden)] = CityId.Vieden
        }.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);

    public static IEndpointRouteBuilder MapTemperatureEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/temperature")
            .RequireAuthorization()
            .WithTags("Temperature");

        group.MapGet("/{city}", GetTemperature)
            .WithName("GetTemperature")
            .WithSummary("Get current temperature")
            .WithDescription(
                 "Returns the latest temperature reading for the given city. " +
                 "Data is sourced from an external WeatherAPI and cached until the next " +
                 "scheduled update window (09:00 or 16:00 UTC). " +
                 "If the upstream API is unavailable, the last known value is returned. " +
                 "Supported cities: bratislava, praha, budapest, vieden.")
            .Produces<TemperatureResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status503ServiceUnavailable);

        return app;
    }

    private static async Task<IResult> GetTemperature(
        [FromRoute] string city,
        ITemperatureService service,
        CancellationToken cancellationToken)
    {
        if (!CityMap.TryGetValue(city, out var cityId))
        {
            return Results.NotFound(new { message = $"City {city} not supported." });
        }

        var result = await service.GetCurrentTemperatureAsync(cityId, cancellationToken);

        if (!result.IsSuccess || result.Value is null)
        {
            return Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
        }

        var reading = result.Value;
        var displayName = CityMap.First(x => x.Value == cityId).Key;

        var response = new TemperatureResponse(
            City: displayName,
            TemperatureC: Math.Round(reading.TemperatureC, 2),
            Description: reading.Description,
            MeasuredAtUtc: reading.MeasuredAtUtc);

        return Results.Ok(response);
    }

    public sealed record TemperatureResponse(
        string City,
        decimal TemperatureC,
        string Description,
        DateTime MeasuredAtUtc);
}
