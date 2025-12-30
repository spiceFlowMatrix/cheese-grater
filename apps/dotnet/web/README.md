# CheeseGrater .NET Backend

ASP.NET Core 9 API secured with Keycloak, delivers SPA config, and uses a compose-run EF migrator to keep the database current.

## How it works
- **EF migrator**: `db-migrator` service (Docker) builds the app image and runs `dotnet ef database update` before the API starts.
- **Keycloak admin client**: seeded via `keycloak-config-cli`; admin client id/secret come from `.env` and allow the API to seed Keycloak (dev).
- **SPA config**: `GET /api/identity/spa-config` returns Keycloak + API settings for the Angular app (no frontend env files).

## Run locally
- Start infra + API via Docker: `docker compose -f docker/docker-compose.yml up --build`
- Or, to run API locally against Docker infra: `dotnet run --project apps/dotnet/web/CheeseGrater.Dotnet.Web.csproj` (after infra is up)
- API: http://localhost:5106
- Swagger UI: http://localhost:5106/api (spec at `/api/specification.json`)

## Environment
- `ConnectionStrings__DefaultConnection`
- `Keycloak__auth-server-url`, `Keycloak__realm`, `Keycloak__resource`, `Keycloak__credentials__secret`
- `KeycloakAdmin__auth-server-url`, `KeycloakAdmin__resource`, `KeycloakAdmin__credentials__secret`
- `SpaClient__ClientId`, `SpaClient__RootUrl`, `SpaClient__RequireHttps`
- `Keycloak__SeedOnStartup` to toggle dev seeding
Defaults live in `.env.example`; override via `.env` / `.env.local`.

## Health
- `/health` used by Docker healthcheck.
