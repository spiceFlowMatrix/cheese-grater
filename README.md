# CheeseGrater

## Purpose of This Monorepo

1. **Reusable Base Platform** – shared infrastructure, auth, and data patterns you can clone into new projects.
2. **Core Service Library** – domain and infrastructure libraries intended for reuse across services.
3. **Showcase & Demo Environment** – runnable Keycloak/Postgres stack plus sample apps to demonstrate the platform.

## Prerequisites

- Docker Desktop
- Node.js, Angular, Nx tooling
- .NET 9 SDK

## Install & Run

- `cd docker && docker compose up -d` (brings up Postgres, Keycloak, pgAdmin)
- Run sample apps via Nx (e.g., `nx serve dotnet-web`)

## Dual IAM Initialization Modes

- **Declarative Mode (recommended)** – GitOps-friendly, reproducible. `KEYCLOAK_BOOTSTRAP_MODE=off` runs `keycloak-config-cli` against `docker/keycloak/config/master-realm.yaml`.
- **Bootstrap Mode (fast dev)** – runs `docker/keycloak/bootstrap/create-admin-client.sh` via `kcadm.sh` to create/update the admin service account client and secret. Enable with `KEYCLOAK_BOOTSTRAP_MODE=on`.

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

## Keycloak Usage Highlights

- UMA and resource-based policies in the .NET backend.
- Service account admin client (`web-admin-serviceaccount`) for Admin API access.
- Deterministic admin client creation (declarative file or bootstrap script) so tokens can be issued during automated seeding and NSwag runs.

## Known Issues

- Nx graph hiccups: run `nx reset` if the graph stalls.
- Nx migrate with `@nx-dotnet/core`: temporarily remove the plugin in `nx.json` if migrate hangs.
- Central Package Management: ensure the file name is exactly `Directory.Packages.props` (case-sensitive on Linux CI).
