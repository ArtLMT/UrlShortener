# =========================
# 1️⃣ BUILD STAGE
# =========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Cài Tailscale (cực nhẹ, chỉ vài MB)
RUN apt-get update && apt-get install -y curl gnupg2 apt-transport-https \
    && curl -fsSL https://pkgs.tailscale.com/stable/debian/bookworm.noarmor.gpg | tee /usr/share/keyrings/tailscale-archive-keyring.gpg >/dev/null \
    && curl -fsSL https://pkgs.tailscale.com/stable/debian/bookworm.tailscale-keyring.list | tee /etc/apt/sources.list.d/tailscale.list \
    && apt-get update && apt-get install -y tailscale iproute2 \
    && rm -rf /var/lib/apt/lists/*

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

CMD ["/bin/sh", "-c", "\
  tailscaled --tun=userspace-networking --socks5-server=localhost:1055 & \
  tailscale up --authkey=$TAILSCALE_AUTHKEY --hostname=urlshortener-api --accept-dns=false --advertise-tags=tag:api && \
  dotnet UrlShortener.Api.dll"]

EXPOSE 5000
EXPOSE 5001

ENTRYPOINT ["dotnet", "UrlShortener.Api.dll"]
