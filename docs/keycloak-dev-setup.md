# Keycloak dev setup for the Angular SPA

## Environment variables

- `KEYCLOAK_SPA_CLIENT_ID` — default: `todo-web`
- `KEYCLOAK_SPA_ROOT_URL` — default: `http://localhost:4200`
- `KEYCLOAK_SPA_REQUIRE_HTTPS` — default: `false`
- `KEYCLOAK_URL` — Keycloak base URL (matches `Keycloak:auth-server-url`)
- `KEYCLOAK_REALM` — realm used by the app (matches `Keycloak:realm`)

The backend seeds both the API client and the SPA client using these values and exposes the SPA auth config at `/api/identity/spa-config`. The Angular app bootstraps by calling this endpoint and initialising Keycloak with the returned values.

## Run locally

1) `docker compose up -d keycloak keycloak-postgres postgres pgadmin` from `docker/`.
2) Run the backend: `nx serve dotnet-web` (or `dotnet watch --project apps/dotnet/web/CheeseGrater.Dotnet.Web.csproj`).
3) Run the Angular app: `nx serve todo` (defaults to http://localhost:4200).
4) Open http://localhost:4200:
   - The SPA calls `/api/identity/spa-config`.
   - Keycloak is initialised with the returned realm/url/client.
   - Visiting a protected route triggers the Keycloak login; on logout, Keycloak redirects to `logoutRedirectUri` from the config.
