# ============================
# 1) Build stage (Izolované prostredie pre kompiláciu)
# ============================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Kopírujeme .csproj súbory zo správnej cesty v repozitári
COPY WeatherNews.slnx .
COPY ["src/WeatherNews.API/WeatherNews.API.csproj", "src/WeatherNews.API/"]
COPY ["src/WeatherNews.Infrastructure/WeatherNews.Infrastructure.csproj", "src/WeatherNews.Infrastructure/"]
COPY ["src/WeatherNews.Application/WeatherNews.Application.csproj", "src/WeatherNews.Application/"]
COPY ["src/WeatherNews.Domain/WeatherNews.Domain.csproj", "src/WeatherNews.Domain/"]

RUN dotnet restore

# Copy remaining sources
COPY . .

# Run tests
RUN dotnet test tests/WeatherNews.Tests/WeatherNews.Tests.csproj \
    --no-restore \
    --configuration Release \
    --logger "console;verbosity=minimal"

# Publish API
RUN dotnet publish src/WeatherNews.API/WeatherNews.API.csproj \
    --no-restore \
    --configuration Release \
    --output /app/publish \
    /p:UseAppHost=false

# ============================
# 2) Runtime stage (Čistý obraz pre produkciu)
# ============================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# curl pre healthcheck
# RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Non-root user for security
RUN useradd -m -u 1000 appuser

#RUN groupadd --system --gid 1001 appgroup \
#  && useradd --system --uid 1001 --gid appgroup --create-home appuser

# Create log directory with correct permissions
RUN mkdir -p /app/logs && chown appuser:appgroup /app/logs

# Copy published output from build stage
COPY --from=build --chown=appuser:appgroup /app/publish .

USER useradd

# Metadata and labels
LABEL org.opencontainers.image.source="https://github.com/ppucik/WeatherNews" \
      org.opencontainers.image.title="WeatherNews API"

ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS="http://+:8080;https://+:8081"
ENV DOTNET_RUNNING_IN_CONTAINER=true

EXPOSE 8080
EXPOSE 8081

# Health check
HEALTHCHECK --interval=30s --timeout=5s --start-period=15s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "WeatherNews.API.dll"]