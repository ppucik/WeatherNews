namespace WeatherNews.Domain.Constants;

using System.Collections.Frozen;
using WeatherNews.Domain.Enums;

public static class CityConstants
{
    public static readonly FrozenDictionary<CityId, int> CityIds =
        new Dictionary<CityId, int>
        {
            [CityId.Bratislava] = 1,
            [CityId.Praha] = 2,
            [CityId.Budapest] = 3,
            [CityId.Vieden] = 4
        }.ToFrozenDictionary();
}
