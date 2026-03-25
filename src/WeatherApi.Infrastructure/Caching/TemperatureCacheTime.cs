namespace WeatherNews.Infrastructure.Caching;

public static class TemperatureCacheTime
{
    public static TimeSpan CalculateTtlUntilNextChange(DateTime nowUtc)
    {
        var today9 = nowUtc.Date.AddHours(9);
        var today16 = nowUtc.Date.AddHours(16);

        DateTime nextChange;

        if (nowUtc < today9)
        {
            nextChange = today9;
        }
        else if (nowUtc < today16)
        {
            nextChange = today16;
        }
        else
        {
            nextChange = today9.AddDays(1);
        }

        return nextChange - nowUtc;
    }
}
