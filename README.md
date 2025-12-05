# CheeseGrater

CheeseGrater is a full-stack reference repo showing a modern **.NET 9 API + Angular SPA** secured with **Keycloak**, backed by **Postgres**, and fully dockerised with deterministic seeding. It’s meant as a portfolio-quality playground and a zero-config starter: clone, `docker compose up`, and everything runs.

## Architecture Overview

- **Infra**: Postgres + PgAdmin, Keycloak with config CLI seeding.
- **Backend**: ASP.NET Core 9 API with Keycloak auth, UMA policies, and SPA config endpoint.
- **Frontend**: Angular SPA that bootstraps Keycloak dynamically from the backend (`/api/identity/spa-config`).

## Quick Start (Docker)

Prereqs: Docker & Docker Compose.

1. `cp .env.example .env`
2. From repo root: `docker compose -f docker/docker-compose.yml up --build`
3. Open:
   - API: http://localhost:5106
   - SPA: http://localhost:4200
   - Keycloak: http://localhost:8081
   - PgAdmin: http://localhost:5050

## Run Backend Locally (uses Docker infra)

1. Start infra: `docker compose up postgres keycloak keycloak-config-cli keycloak-db pgadmin`
2. From repo root: `dotnet run --project apps/dotnet/web/CheeseGrater.Dotnet.Web.csproj`
3. Backend reads DB/Keycloak from env (`.env`), so no JSON edits required.

## Run Frontend Locally

1. Ensure backend is running (Docker or local).
2. Install deps: `npm install` (from repo root).
3. `npx nx serve todo` (SPA runs at http://localhost:4200).
4. The SPA calls `/api/identity/spa-config`; no hardcoded API/Keycloak URLs.

## Configuration

- `.env.example` lists all defaults; copy to `.env` to override.
- Backend envs: DB connection, Keycloak (realm, admin client, SPA client), seeding toggle.
- Frontend gets its Keycloak/API details from the backend endpoint—no environment files to edit.

## Known Limitations / Open Issues

- HTTPS termination not configured in Docker examples.
- Angular container uses a basic Nginx proxy; adjust if you need SSR.
- Additional environments (staging/prod) are not templated yet.
