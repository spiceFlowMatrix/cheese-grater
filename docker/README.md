# Docker Stack

Run all infra and app containers from this directory using `docker-compose.yml`.

## Quick Start

From repo root:

```
cp .env.example .env
docker compose -f docker/docker-compose.yml up --build
```

Services exposed:
- API: http://localhost:5106
- SPA: http://localhost:4200
- Keycloak: http://localhost:8081
- PgAdmin: http://localhost:5050

## Services
- `postgres` (app DB) with healthcheck
- `pgadmin` (DB inspection)
- `keycloak-db` (Keycloak DB) with healthcheck
- `keycloak` (realm import + bootstrap hook) with `/health/ready` healthcheck
- `keycloak-config-cli` (one-shot declarative import via keycloak-config-cli)
- `api` (ASP.NET Core) depends on DB + Keycloak + config CLI; health `/health`
- `web` (Angular via Nginx) depends on API; health `/`

## Paths & Build Contexts
- Compose file: `docker/docker-compose.yml` (only compose file)
- API Dockerfile: `../apps/dotnet/web/Dockerfile` (relative to this directory)
- SPA Dockerfile: `../apps/todo/Dockerfile`
- Keycloak config CLI: `./keycloak/config/Dockerfile`
- Keycloak providers/imports: `./keycloak/providers`, `./keycloak/imports`

## IAM Initialization Modes
- **Declarative Mode (default)**: `KEYCLOAK_BOOTSTRAP_MODE=off` renders `keycloak/config/master-realm.yaml.template` and runs keycloak-config-cli.
- **Bootstrap Mode (fast dev)**: `KEYCLOAK_BOOTSTRAP_MODE=on` executes `/opt/keycloak/bootstrap/create-admin-client.sh` inside Keycloak to (re)create `web-admin-serviceaccount`.

## Env
- Use the root `.env.example` as the source of truth. Copy to `.env` at repo root before running compose.

## Notes
- All services share the default network; hostnames match service names (`postgres`, `keycloak`, `api`, `web`).
- Healthcheck-based `depends_on` ensures startup order.
