using Grpc.Core;
using WeatherNews.API.Protos;
using WeatherNews.Application.Abstractions;
using WeatherNews.Domain.Constants;

namespace WeatherNews.API.Services;

public class WeatherGrpcService : WeatherService.WeatherServiceBase
{
    private readonly ITemperatureService _temperatureService;

    public WeatherGrpcService(ITemperatureService temperatureService)
    {
        _temperatureService = temperatureService;
    }

    public override async Task<TemperatureResponse> GetTemperature(
        TemperatureRequest request,
        ServerCallContext context)
    {
        if (!CityConstants.CityMap.TryGetValue(request.City, out var cityId))
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"City {request.City} not supported"));
        }

        //var cityId = new CityId(request.City);
        var result = await _temperatureService.GetCurrentTemperatureAsync(cityId, context.CancellationToken);

        if (!result.IsSuccess)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"City {request.City} not found"));
        }

        // Mapovanie z vášho Domain Recordu na gRPC Message
        var reading = result.Value!;

        return new TemperatureResponse
        {
            City = reading.CityId.ToString(),
            TemperatureC = (double)reading.TemperatureC,
            Description = reading.Description,
            MeasuredAtUtc = reading.MeasuredAtUtc.ToString("O")
        };
    }
}