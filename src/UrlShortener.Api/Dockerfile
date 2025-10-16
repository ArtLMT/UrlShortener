# =========================
# 1️⃣ BUILD STAGE
# =========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution và csproj để restore
COPY ["UrlShortener.sln", "./"]
COPY ["src/UrlShortener.Api/UrlShortener.Api.csproj", "src/UrlShortener.Api/"]
COPY ["src/UrlShortener.Infrastructure/UrlShortener.Infrastructure.csproj", "src/UrlShortener.Infrastructure/"]
COPY ["src/UrlShortener.Application/UrlShortener.Application.csproj", "src/UrlShortener.Application/"]
COPY ["src/UrlShortener.Domain/UrlShortener.Domain.csproj", "src/UrlShortener.Domain/"]
COPY ["UrlShortener.Tests/UrlShortener.Tests.csproj", "UrlShortener.Tests/"]

# Restore dependencies
RUN dotnet restore "src/UrlShortener.Api/UrlShortener.Api.csproj"

# Copy toàn bộ source code
COPY src/ ./src/
COPY UrlShortener.Tests/ ./UrlShortener.Tests/

# Build Web API
RUN dotnet build "src/UrlShortener.Api/UrlShortener.Api.csproj" -c Release -o /build

# Run tests
RUN dotnet test "UrlShortener.Tests/UrlShortener.Tests.csproj" --no-build --verbosity normal

# Publish Web API
RUN dotnet publish "src/UrlShortener.Api/UrlShortener.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# =========================
# 2️⃣ RUNTIME STAGE
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy published output
COPY --from=build /app/publish .

EXPOSE 5000
EXPOSE 5001

ENTRYPOINT ["dotnet", "UrlShortener.Api.dll"]
