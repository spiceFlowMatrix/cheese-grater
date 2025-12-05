# Todo Angular App

Angular SPA demonstrating Keycloak auth via dynamic config from the backend.

## Purpose
- Simple UI shell with auth redirect flow and header.
- Fetches Keycloak + API config from `/api/identity/spa-config` at startup.
- No hardcoded API or Keycloak URLs in the code.

## Run Locally
1. Ensure backend + infra are running (Docker `docker compose up` or local backend).
2. Install deps: `npm install` (from repo root).
3. `npx nx serve todo` (defaults to http://localhost:4200).

## Configuration
- SPA bootstraps via the backend endpoint; no Angular environment files required.
- Keycloak client ID / redirect origins come from the backend `SpaClient` options.

## Docker Notes
- Served by the `web` service in `docker-compose.yml` (Nginx).
- `/api` is proxied to the `api` service inside the Docker network.

## Auth Flow
- Header links to `/auth-redirect`.
- Auth redirect page shows login or logout buttons based on Keycloak state and calls Keycloak JS login/logout.
