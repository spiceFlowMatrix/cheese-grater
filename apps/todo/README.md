# Todo Angular App

Angular SPA that bootstraps Keycloak + API settings from the backend (no Angular env files).

## How it works
- Fetches `/api/identity/spa-config` at startup to configure Keycloak and API base URL.
- Auth flows use the SPA client seeded in Keycloak (via config CLI).

## Run locally
1) Start infra + API: `docker compose -f docker/docker-compose.yml up --build`
2) `yarn install`
3) `yarn nx serve todo` (http://localhost:4200)

## Dockerized deployment
- Built via `apps/todo/Dockerfile` (nginx runtime).  
- Healthcheck uses `curl` on `/`; `/api` should be proxied to the API service in compose.
