# ============================
# 1) Build stage (Izolované prostredie pre kompiláciu)
# ============================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /build 

# Kopírujeme .csproj súbory zo správnej cesty v repozitári
COPY ["src/WeatherNews.API/WeatherNews.API.csproj", "src/WeatherNews.API/"]
COPY ["src/WeatherNews.Infrastructure/WeatherNews.Infrastructure.csproj", "src/WeatherNews.Infrastructure/"]
COPY ["src/WeatherNews.Application/WeatherNews.Application.csproj", "src/WeatherNews.Application/"]
COPY ["src/WeatherNews.Domain/WeatherNews.Domain.csproj", "src/WeatherNews.Domain/"]

RUN dotnet restore "src/WeatherNews.API/WeatherNews.API.csproj"

# Skopíruje celý obsah repozitára do /build v kontajneri
COPY . . 

# Prepne sa do priečinka projektu
WORKDIR "/build/src/WeatherNews.API"
RUN dotnet publish "WeatherNews.API.csproj" -c Release -o /app/publish --no-restore 

# ============================
# 2) Runtime stage (Čistý obraz pre produkciu)
# ============================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# curl pre healthcheck
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Bezpečnostný užívateľ
RUN useradd -m -u 1000 appuser

# Kopírujeme LEN výsledok buildu (z /app/publish do aktuálneho /app)
COPY --from=build /app/publish .
RUN chown -R appuser:appuser /app

# Metadata a prostredie
LABEL org.opencontainers.image.source="https://github.com/ppucik/WeatherNews" \
      org.opencontainers.image.title="WeatherNews API"

EXPOSE 8080
EXPOSE 8081

ENV ASPNETCORE_URLS="http://+:8080;https://+:8081"
ENV ASPNETCORE_ENVIRONMENT=Production

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD curl -f http://localhost:8080/health || exit 1

USER appuser
ENTRYPOINT ["dotnet", "WeatherNews.API.dll"]