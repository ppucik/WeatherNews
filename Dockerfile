# ============================
# 1) Build stage
# ============================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore
COPY ["WeatherNews.API/WeatherNews.API.csproj", "WeatherNews.API/"]
COPY ["WeatherNews.Domain/WeatherNews.Domain.csproj", "WeatherNews.Domain/"]
COPY ["WeatherNews.Infrastructure/WeatherNews.Infrastructure.csproj", "WeatherNews.Infrastructure/"]

RUN dotnet restore "WeatherNews.API/WeatherNews.API.csproj"

# Copy everything and build
COPY . .
WORKDIR "/src/WeatherNews.API"
RUN dotnet publish "WeatherNews.API.csproj" -c Release -o /app/publish --no-restore

# ============================
# 2) Runtime stage
# ============================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copy published output
COPY --from=build /app/publish .

# Expose ports
EXPOSE 8080
EXPOSE 8081

# Environment variables for Kestrel
ENV ASPNETCORE_URLS="http://+:8080;https://+:8081"
ENV ASPNETCORE_ENVIRONMENT=Development

# Run the app
ENTRYPOINT ["dotnet", "WeatherNews.API.dll"]
