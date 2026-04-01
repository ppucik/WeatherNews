namespace WeatherNews.Application.Abstractions;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}
