FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and csproj files for proper Docker layer caching
COPY cheese-grater.sln ./

COPY apps/dotnet/web/*.csproj apps/dotnet/web/
COPY apps/dotnet/web-test/*.csproj apps/dotnet/web-test/
COPY libs/dotnet-packages-aggregate/*.csproj libs/dotnet-packages-aggregate/
COPY libs/dotnet/application/*.csproj libs/dotnet/application/
COPY libs/dotnet/infrastructure/*.csproj libs/dotnet/infrastructure/
COPY libs/dotnet/core/application/*.csproj libs/dotnet/core/application/
COPY libs/dotnet/core/domain/*.csproj libs/dotnet/core/domain/
COPY libs/dotnet/core/infrastructure/*.csproj libs/dotnet/core/infrastructure/

# Skip Nx module boundary checks inside Docker (node CLI isn't available here)
ENV NX_DOTNET_SKIP_MODULE_BOUNDARIES=true

# Restore dependencies
RUN dotnet restore

# Copy the rest of the repository
COPY . .

# Publish (skip NSwag generation)
RUN dotnet publish apps/dotnet/web/CheeseGrater.Dotnet.Web.csproj \
    -c Release \
    -o /app/publish \
    /p:SkipNSwag=True

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://0.0.0.0:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "CheeseGrater.Web.dll"]
