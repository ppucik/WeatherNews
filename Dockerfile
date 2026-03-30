# ============================
# 1) Build stage
# ============================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj and restore
COPY ["WeatherNews.API/WeatherNews.API.csproj", "WeatherNews.API/"]
COPY ["WeatherNews.Infrastructure/WeatherNews.Infrastructure.csproj", "WeatherNews.Infrastructure/"]
COPY ["WeatherNews.Application/WeatherNews.Application.csproj", "WeatherNews.Application/"]
COPY ["WeatherNews.Domain/WeatherNews.Domain.csproj", "WeatherNews.Domain/"]

RUN dotnet restore "WeatherNews.API/WeatherNews.API.csproj"

# Copy everything and build
COPY . .
WORKDIR "/src/WeatherNews.API"
RUN dotnet publish "WeatherNews.API.csproj" -c Release -o /app/publish --no-restore

# ============================
# 2) Runtime stage
# ============================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Create non-root user for security
RUN useradd -m -u 1000 appuser

# Copy published output
COPY --from=build /app/publish .
RUN chown -R appuser:appuser /app

# Metadata labels
LABEL org.opencontainers.image.source="https://github.com/ppucik/WeatherNews" \
      org.opencontainers.image.title="WeatherNews API" \
      org.opencontainers.image.description="Weather News API Service"

# Expose ports
EXPOSE 8080
EXPOSE 8081

# Environment variables for Kestrel
ENV ASPNETCORE_URLS="http://+:8080;https://+:8081"
ENV ASPNETCORE_ENVIRONMENT=Production

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD curl -f http://localhost:8080/health || exit 1

# Switch to non-root user
USER appuser

# Run the app
ENTRYPOINT ["dotnet", "WeatherNews.API.dll"]