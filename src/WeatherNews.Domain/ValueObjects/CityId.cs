/*
using System.Collections.Frozen;
using WeatherNews.Domain.Common;

namespace WeatherNews.Domain.ValueObjects;

public sealed record CityId(int Id, string Name)
{
    // Definícia statických inštancií so zachovaním pôvodných ID
    public static readonly CityId Bratislava = new(1, "Bratislava");
    public static readonly CityId Praha = new(2, "Praha");
    public static readonly CityId Budapest = new(3, "Budapest");
    public static readonly CityId Vieden = new(4, "Vieden");

    // Mapa pre rýchle vyhľadávanie podľa názvu (pre API/gRPC vstupy)
    private static readonly FrozenDictionary<string, CityId> ByName =
        new Dictionary<string, CityId>(StringComparer.OrdinalIgnoreCase)
        {
            [Bratislava.Name] = Bratislava,
            [Praha.Name] = Praha,
            [Budapest.Name] = Budapest,
            [Vieden.Name] = Vieden
        }.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);

    // Mapa pre vyhľadávanie podľa ID (pre databázu/EF Core)
    private static readonly FrozenDictionary<int, CityId> ById =
        ByName.Values.ToFrozenDictionary(c => c.Id);

    public static Result<CityId> FromString(string name) =>
        ByName.TryGetValue(name, out var city)
            ? Result<CityId>.Success(city)
            : Result<CityId>.Failure($"Mesto '{name}' nie je podporované.");

    public static Result<CityId> FromId(int id) =>
        ById.TryGetValue(id, out var city)
            ? Result<CityId>.Success(city)
            : Result<CityId>.Failure($"ID mesta {id} neexistuje.");
}
*/