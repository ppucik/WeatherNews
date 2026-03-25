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
            ["bratislava"] = CityId.Bratislava,
            ["praha"] = CityId.Praha,
            ["budapest"] = CityId.Budapest,
            ["vieden"] = CityId.Vieden
        }.ToFrozenDictionary();

    public static IEndpointRouteBuilder MapTemperatureEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/temperature")
            .RequireAuthorization()
            .WithTags("Temperature");

        group.MapGet("/{city}", GetTemperature)
            .WithName("GetTemperature")
            .WithSummary("Get current temperature for a city")
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
        if (!CityMap.TryGetValue(city, out var cityEnum))
        {
            return Results.NotFound(new { message = "City not supported." });
        }

        var result = await service.GetCurrentTemperatureAsync(cityEnum, cancellationToken);

        if (!result.IsSuccess || result.Value is null)
        {
            return Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
        }

        var reading = result.Value;
        var response = new TemperatureResponse(
            City: city.ToLowerInvariant(),
            TemperatureC: Math.Round(reading.TemperatureC, 2),
            MeasuredAtUtc: reading.MeasuredAtUtc);

        return Results.Ok(response);
    }

    public sealed record TemperatureResponse(
        string City,
        decimal TemperatureC,
        DateTime MeasuredAtUtc);
}
