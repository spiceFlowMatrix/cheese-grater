# Docker Stack & Identity Modes

## Purpose of This Monorepo
1) **Reusable Base Platform** – shared infra/auth templates for new projects.  
2) **Core Service Library** – common domain/infrastructure packages.  
3) **Showcase & Demo Environment** – runnable Keycloak/Postgres stack for demos.

## Dual IAM Initialization Modes
- **Declarative Mode (recommended)**  
  - `KEYCLOAK_BOOTSTRAP_MODE=off`  
  - Runs `adorsys/keycloak-config-cli` against `docker/keycloak/config/master-realm.yaml` (deterministic admin client + secret).
- **Bootstrap Mode (fast dev)**  
  - `KEYCLOAK_BOOTSTRAP_MODE=on`  
  - Keycloak container runs `/opt/keycloak/bootstrap/create-admin-client.sh` (kcadm-based, idempotent) to create/update `web-admin-serviceaccount`, set its secret, audience mapper, and admin role.

### Architecture (ASCII)
```
Monorepo
 ├── Core Libraries
 ├── Sample Apps
 ├── Identity Infrastructure
 │     ├── Declarative Mode
 │     └── Bootstrap Mode
 └── Docker Stack
```

## Services
- `postgres` / `keycloak-postgres` (data under `/var/lib/postgresql`)
- `keycloak` with realm import + bootstrap hook
- `keycloak-config` (one-shot declarative import when bootstrap is off)
- `pgadmin` for DB inspection

## Key Files
- Declarative config: `docker/keycloak/config/master-realm.yaml`
- Bootstrap script: `docker/keycloak/bootstrap/create-admin-client.sh`
- Compose: `docker/docker-compose.yaml`
- Env template: `.env.example` (real `.env` is gitignored)

## Keycloak Usage
- Service account admin client (`web-admin-serviceaccount`) with deterministic secret for Admin API calls.
- Audience mapper for `security-admin-console`.
- Admin realm role assigned to the service account.
