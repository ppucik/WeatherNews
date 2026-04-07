namespace WeatherNews.Domain.Constants;

using System.Collections.Frozen;
using WeatherNews.Domain.Enums;

public static class CityConstants
{
    public static readonly FrozenDictionary<string, CityId> CityMap =
        new Dictionary<string, CityId>(StringComparer.OrdinalIgnoreCase)
        {
            [nameof(CityId.Bratislava)] = CityId.Bratislava,
            [nameof(CityId.Praha)] = CityId.Praha,
            [nameof(CityId.Budapest)] = CityId.Budapest,
            [nameof(CityId.Vieden)] = CityId.Vieden
        }.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);
}
