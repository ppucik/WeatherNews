//using Moq;
//using WeatherNews.Application.Abstractions;
//using WeatherNews.Domain.Common;
//using WeatherNews.Domain.Entities;
//using WeatherNews.Domain.Enums;

//namespace WeatherNews.Tests.gRPC;

//public class WeatherGrpcServiceTests
//{
//    [Fact]
//    public async Task GetTemperature_ShouldReturnResponse_WhenCityIsValid()
//    {
//        // Arrange
//        var mockService = new Mock<ITemperatureService>();
//        var reading = new TemperatureReading(CityId.Bratislava, 20.5m, "Clear", DateTime.UtcNow);

//        mockService.Setup(s => s.GetCurrentTemperatureAsync(CityId.Bratislava, It.IsAny<CancellationToken>()))
//                   .ReturnsAsync(Result<TemperatureReading>.Success(reading));

//        var grpcService = new WeatherGrpcService(mockService.Object);
//        var request = new TemperatureRequest { City = "Bratislava" };

//        // Act
//        var response = await grpcService.GetTemperature(request, null!);

//        // Assert
//        Assert.Equal(20.5, response.Temperature);
//        Assert.Equal("Bratislava", response.CityName);
//    }
//}