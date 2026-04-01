# ============================
# 1) Build stage
# ============================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj and restore
COPY ["src/WeatherNews.API/WeatherNews.API.csproj", "src/WeatherNews.API/"]
COPY ["src/WeatherNews.Infrastructure/WeatherNews.Infrastructure.csproj", "src/WeatherNews.Infrastructure/"]
COPY ["src/WeatherNews.Application/WeatherNews.Application.csproj", "src/WeatherNews.Application/"]
COPY ["src/WeatherNews.Domain/WeatherNews.Domain.csproj", "src/WeatherNews.Domain/"]

RUN dotnet restore "src/WeatherNews.API/WeatherNews.API.csproj"

# Copy everything and build
COPY . .

WORKDIR "/src/WeatherApi.Application"
RUN dotnet publish "WeatherApi.Application.csproj" -c Release -o /app/publish --no-restore

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

# Switch to non-root user
USER appuser

# Run the app
ENTRYPOINT ["dotnet", "WeatherApi.Application.dll"]