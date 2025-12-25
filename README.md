# CheeseGrater

Deterministic full-stack starter showcasing **.NET 9** + **Angular** with **Keycloak** and **Postgres**, wired for reproducible local infra, EF migrations via a one-shot migrator, and monorepo-friendly Docker/Nx workflows.

## What’s inside

- **Infra**: Postgres + PgAdmin, Keycloak with declarative seeding (config CLI).
- **Backend**: ASP.NET Core 9 API with Keycloak auth/UMA and SPA config at `/api/identity/spa-config`.
- **Frontend**: Angular SPA that bootstraps Keycloak/API config from the backend (no hardcoded env files).

## Quick start (Docker infra + API)

Prereqs: Docker/Compose. Run from repo root:

1. `cp .env.example .env` (or use `.env.local` for personal overrides)
2. `docker compose -f docker/docker-compose.yml up --build`
3. URLs: API http://localhost:5106 (Swagger UI at `/api`, spec at `/api/specification.json`), Keycloak http://localhost:8081, PgAdmin http://localhost:5050.

## Local development

- **Infra + API via Docker**: `docker compose -f docker/docker-compose.yml up --build` (includes EF `db-migrator`, Postgres, Keycloak, API).
- **Frontend locally**: `yarn install` then `yarn nx serve todo` (http://localhost:4200). The SPA reads `/api/identity/spa-config`; no Angular env files needed.

## Environment overrides

- Copy `.env.example` to `.env` for Docker defaults; use `.env.local` for local-only overrides.
- Key vars: `ConnectionStrings__DefaultConnection`, Keycloak realm/admin/client secrets, SPA client IDs/URLs. Defaults are in `.env.example`.

## Notes

- EF migrations run via the `db-migrator` one-shot service in Compose before the API starts.
- HTTPS/prod hardening is out of scope; adjust compose/nginx as needed.
