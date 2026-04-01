# ============================
# 1) Build stage (Izolované prostredie pre kompiláciu)
# ============================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Optimalizácia cache: Kopírujeme súbory riešenia a projektov samostatne 
COPY ["Directory.Build.props", "."]
COPY ["Directory.Packages.props", "."]
COPY ["docker-compose.dcproj", "."]
COPY ["WeatherNews.slnx", "."]

COPY ["src/WeatherNews.API/WeatherNews.API.csproj", "src/WeatherNews.API/"]
COPY ["src/WeatherNews.Infrastructure/WeatherNews.Infrastructure.csproj", "src/WeatherNews.Infrastructure/"]
COPY ["src/WeatherNews.Application/WeatherNews.Application.csproj", "src/WeatherNews.Application/"]
COPY ["src/WeatherNews.Domain/WeatherNews.Domain.csproj", "src/WeatherNews.Domain/"]
COPY ["tests/WeatherNews.Tests/WeatherNews.Tests.csproj", "tests/WeatherNews.Tests/"]

# Obnova závislostí cielene pre solution file 
RUN dotnet restore "WeatherNews.slnx"

# Kopírovanie zvyšných zdrojových kódov - opravený spojený riadok 
COPY . .

# Spustenie testov pred publikáciou [cite: 2]
RUN dotnet test "tests/WeatherNews.Tests/WeatherNews.Tests.csproj" \
    --no-restore \
    --configuration $BUILD_CONFIGURATION \
    --logger "console;verbosity=minimal"

# Publikácia API projektu [cite: 2]
RUN dotnet publish "src/WeatherNews.API/WeatherNews.API.csproj" \
    --no-restore \
    --configuration $BUILD_CONFIGURATION \
    --output /app/publish \
    /p:UseAppHost=false

# ============================
# 2) Runtime stage (Čistý obraz pre produkciu)
# ============================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Inštalácia curl pre healthcheck s čistením cache pre menší obraz 
RUN apt-get update && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

# Vytvorenie non-root používateľa pre vyššiu bezpečnosť [cite: 3]
RUN groupadd --system --gid 1001 appgroup \
  && useradd --system --uid 1001 --gid appgroup --create-home appuser

# Príprava priečinka pre logy so správnymi právami [cite: 3]
RUN mkdir -p /app/logs && chown appuser:appgroup /app/logs

# Kopírovanie publikovaného výstupu s okamžitým priradením vlastníka [cite: 4]
COPY --from=build --chown=appuser:appgroup /app/publish .

# Prepnutie na ne-root používateľa [cite: 4]
USER appuser

# Metadáta obrazu [cite: 4]
LABEL org.opencontainers.image.source="https://github.com/ppucik/WeatherNews" \
      org.opencontainers.image.title="WeatherNews API"

ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS="http://+:8080"
ENV DOTNET_RUNNING_IN_CONTAINER=true

EXPOSE 8080

# Health check na overenie stavu aplikácie 
HEALTHCHECK --interval=30s --timeout=5s --start-period=15s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "WeatherNews.API.dll"]