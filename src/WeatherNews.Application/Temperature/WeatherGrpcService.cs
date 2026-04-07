//namespace WeatherNews.Application.Temperature;

//public class WeatherGrpcService : WeatherService.WeatherServiceBase
//{
//    private readonly ITemperatureService _temperatureService;

//    // Injectujeme vašu hotovú biznis logiku
//    public WeatherGrpcService(ITemperatureService temperatureService)
//    {
//        _temperatureService = temperatureService;
//    }

//    public override async Task<TemperatureResponse> GetTemperature(
//        TemperatureRequest request, ServerCallContext context)
//    {
//        // 1. Preklad z gRPC Requestu na doménový typ (CityId)
//        var cityId = new CityId(request.City);

//        // 2. Zavolanie existujúcej logiky (využíva cache, providera aj logger)
//        var result = await _temperatureService.GetCurrentTemperatureAsync(cityId, context.CancellationToken);

//        if (!result.IsSuccess)
//        {
//            throw new RpcException(new Status(StatusCode.NotFound, result.Error));
//        }

//        // 3. Preklad výsledku späť na gRPC Response (Protobuf)
//        return new TemperatureResponse
//        {
//            City = result.Value.ToString(),
//            TemperatureC = (double)result.Value.TemperatureC,
//            Description = result.Value!.Description,
//            MeasuredAtUtc = "Celsius"
//        };
//    }
//}
