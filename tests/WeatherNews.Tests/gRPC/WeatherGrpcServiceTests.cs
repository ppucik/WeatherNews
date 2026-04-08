using Grpc.Core;
using Moq;
using WeatherNews.API.Protos;
using WeatherNews.API.Services;
using WeatherNews.Application.Abstractions;
using WeatherNews.Domain.Common;
using WeatherNews.Domain.Entities;
using WeatherNews.Domain.Enums;

namespace WeatherNews.Tests.gRPC;

public class WeatherGrpcServiceTests
{
    [Fact]
    public async Task GetTemperature_ShouldReturnResponse_WhenCityIsValid()
    {
        // Arrange
        var mockService = new Mock<ITemperatureService>();

        var reading = new TemperatureReading(
            CityId.Bratislava,
            20.5m,
            "Clear",
            DateTime.UtcNow
        );

        mockService.Setup(s => s.GetCurrentTemperatureAsync(
            CityId.Bratislava,
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<TemperatureReading>.Success(reading));

        var grpcService = new WeatherGrpcService(mockService.Object);
        var request = new TemperatureRequest { City = "Bratislava" };

        // Act
        var response = await grpcService.GetTemperature(request, CreateMockContext());

        // Assert
        Assert.Equal(20.5, response.TemperatureC);
        Assert.Equal("Bratislava", response.City);
    }

    [Fact]
    public async Task GetTemperature_ShouldThrowRpcException_WithNotFound_WhenCityIsNotSupported()
    {
        // Arrange
        var mockService = new Mock<ITemperatureService>();
        var grpcService = new WeatherGrpcService(mockService.Object);

        // request s mestom, ktoré nie je v CityConstants.CityMap
        var request = new TemperatureRequest { City = "Narnia" };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(() =>
            grpcService.GetTemperature(request, CreateMockContext()));

        Assert.Equal(StatusCode.NotFound, exception.Status.StatusCode);
        Assert.Contains("not supported", exception.Status.Detail);
    }

    // Pomocná metóda na vytvorenie kontextu
    private static ServerCallContext CreateMockContext()
    {
        return new Mock<ServerCallContext>().Object;
    }
}