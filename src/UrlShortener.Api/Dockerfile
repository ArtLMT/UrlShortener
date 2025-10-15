# =========================
# 1️⃣ BUILD STAGE
# =========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy các csproj để restore
COPY ["UrlShortener.Api/UrlShortener.Api.csproj", "UrlShortener.Api/"]
COPY ["UrlShortener.Infrastructure/UrlShortener.Infrastructure.csproj", "UrlShortener.Infrastructure/"]
COPY ["UrlShortener.Application/UrlShortener.Application.csproj", "UrlShortener.Application/"]
COPY ["UrlShortener.Domain/UrlShortener.Domain.csproj", "UrlShortener.Domain/"]

# Restore dependencies
RUN dotnet restore "UrlShortener.Api/UrlShortener.Api.csproj"

# Copy full source
COPY . .

# Build & publish
WORKDIR /src/UrlShortener.Api
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# =========================
# 2️⃣ RUNTIME STAGE
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy published output
COPY --from=build /app/publish .

# Expose ports (for HTTP & HTTPS)
EXPOSE 5000
EXPOSE 5001

# Run the API
ENTRYPOINT ["dotnet", "UrlShortener.Api.dll"]
