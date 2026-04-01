# ============================
# 1) Build stage
# ============================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj and restore
COPY ["src/WeatherApi.Application/WeatherApi.Application.csproj", "WeatherApi.Application/"]
COPY ["src/WeatherApi.Infrastructure/WeatherApi.Infrastructure.csproj", "WeatherApi.Infrastructure/"]
COPY ["src/WeatherNews.Domain/WeatherNews.Domain.csproj", "WeatherNews.Domain/"]

RUN dotnet restore "WeatherApi.Application/WeatherApi.Application.csproj"

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