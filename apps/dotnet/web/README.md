# CheeseGrater .NET Backend

Primary ASP.NET Core 9 API with Keycloak integration and SPA config delivery.

## Purpose
- Demo API with Clean Architecture layers and UMA/Keycloak integration.
- Exposes `/api/identity/spa-config` for the Angular SPA to bootstrap Keycloak.
- Seeds Keycloak (dev only) for API + SPA clients.

## Run Locally (connects to Docker infra)
1. Start infra: `docker compose -f docker/docker-compose.yml up postgres keycloak keycloak-config-cli keycloak-db pgadmin`
2. From repo root: `dotnet run --project apps/dotnet/web/CheeseGrater.Dotnet.Web.csproj`
3. Open API: `http://localhost:5106` (or your configured `API_HTTP_PORT`).

## Keycloak Integration
- Realm: `Test` (default)
- API client: `test-client` (confidential; secret via env)
- SPA client: `todo-web` (public; seeded with redirects/web origins from `SpaClient` options)
- SPA config endpoint: `GET /api/identity/spa-config`
- Seeding: Development only; controlled by `Keycloak:SeedOnStartup` (or env `KEYCLOAK__SEEDONSTARTUP`).

## Environment Variables (common)
- `ConnectionStrings__DefaultConnection` (e.g., `Host=postgres;Port=5432;...`)
- `Keycloak__auth-server-url` (docker: `http://keycloak:8080/`, local: `http://localhost:8081/`)
- `Keycloak__realm`, `Keycloak__resource`, `Keycloak__credentials__secret`
- `KeycloakAdmin__auth-server-url`, `KeycloakAdmin__resource`, `KeycloakAdmin__credentials__secret`
- `SpaClient__ClientId`, `SpaClient__RootUrl`, `SpaClient__RequireHttps`
- `Keycloak:SeedOnStartup` (bool)

See root `.env.example` for defaults.

## Health
- Health endpoint: `/health`
- Docker healthcheck uses this path.
